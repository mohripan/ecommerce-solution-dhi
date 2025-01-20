using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs.Requests;
using TransactionService.Application.DTOs.Responses;
using TransactionService.Application.Exceptions;
using TransactionService.Application.Services.Interfaces;
using TransactionService.Contracts.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Factories;

namespace TransactionService.Application.Services.Impls
{
    public class TransactionHistoryAppService : ITransactionHistoryAppService
    {
        private readonly ITransactionHistoryRepository _transactionHistoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionHistoryFactory _transactionHistoryFactory;
        private readonly IProductServiceClient _productServiceClient;

        public TransactionHistoryAppService(
            ITransactionHistoryRepository transactionHistoryRepository,
            IUnitOfWork unitOfWork,
            ITransactionHistoryFactory transactionHistoryFactory,
            IProductServiceClient productServiceClient)
        {
            _transactionHistoryRepository = transactionHistoryRepository;
            _unitOfWork = unitOfWork;
            _transactionHistoryFactory = transactionHistoryFactory;
            _productServiceClient = productServiceClient;
        }

        public async Task<TransactionHistoryResponseDto> CreateTransactionAsync(TransactionHistoryRequestDto requestDto, int userId)
        {
            if (userId <= 0)
                throw new GlobalException("Invalid UserId.");

            var transaction = _transactionHistoryFactory.CreateTransaction(
                requestDto.ProductId,
                userId,
                requestDto.Quantity,
                requestDto.Price,
                requestDto.Remarks ?? string.Empty
            );

            await _transactionHistoryRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transaction);
        }

        public async Task<TransactionHistoryResponseDto?> UpdateTransactionStatusAsync(int id, int statusId, string bearerToken)
        {
            var transaction = await _transactionHistoryRepository.GetByIdAsync(id);
            if (transaction == null) throw new GlobalException("Transaction not found.");

            if (transaction.StatusId == 3 && statusId == 2)
                throw new GlobalException("Cannot change status from Cancelled to Completed.");
            if (transaction.StatusId == 2 && statusId == 3)
                throw new GlobalException("Cannot change status from Completed to Cancelled.");

            if (statusId == 2)
            {
                if (string.IsNullOrWhiteSpace(bearerToken))
                {
                    throw new GlobalException("Cannot complete transaction: missing Bearer token to call product service.");
                }

                var currentQuantity = await _productServiceClient.GetProductQuantityAsync(bearerToken, transaction.ProductId);

                if (currentQuantity < transaction.Quantity)
                {
                    throw new GlobalException("Cannot complete transaction: insufficient product quantity.");
                }

                bool success = await _productServiceClient.MarkProductAsPurchasedAsync(
                    bearerToken,
                    transaction.ProductId,
                    transaction.Quantity
                );
                if (!success)
                {
                    throw new GlobalException("Failed to mark product as purchased in remote service. Cannot complete transaction.");
                }
            }

            transaction.StatusId = statusId;
            transaction.ModifiedOn = DateTime.UtcNow;

            await _transactionHistoryRepository.UpdateStatusAsync(id, statusId);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transaction);
        }

        public async Task<SellerProductTransactionPaginatedResponse>
        GetSellerProductTransactionsAsync(int sellerId, string bearerToken, int page, int sizePerPage)
        {
            var (totalItems, products) =
                await _productServiceClient.GetSellerProductsAsync(bearerToken, page, sizePerPage);

            if (!products.Any())
            {
                return new SellerProductTransactionPaginatedResponse
                {
                    Search = new PaginationMetadata
                    {
                        Total = totalItems,
                        TotalPage = (int)Math.Ceiling((double)totalItems / sizePerPage),
                        SizePerPage = sizePerPage,
                        PageAt = page
                    },
                    Values = new List<SellerProductTransactionRequestDto>()
                };
            }

            var productIds = products.Select(p => p.productId).ToList();
            var allTransactions = await _transactionHistoryRepository.GetByProductIdsAsync(productIds);

            var grouped = allTransactions
                .GroupBy(t => t.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var listDtos = new List<SellerProductTransactionRequestDto>();
            foreach (var (productId, productName) in products)
            {
                var transactionList = new List<TransactionHistoryResponseDto>();
                if (grouped.ContainsKey(productId))
                {
                    transactionList = grouped[productId]
                        .Select(MapToResponse)
                        .ToList();
                }

                listDtos.Add(new SellerProductTransactionRequestDto
                {
                    ProductId = productId,
                    ProductName = productName,
                    TransactionHistory = transactionList
                });
            }

            var totalPage = (int)Math.Ceiling((double)totalItems / sizePerPage);
            return new SellerProductTransactionPaginatedResponse
            {
                Search = new PaginationMetadata
                {
                    Total = totalItems,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = listDtos
            };
        }

        public async Task<TransactionHistoryResponseDto?> GetTransactionByIdAsync(int id, string bearerToken)
        {
            var transaction = await _transactionHistoryRepository.GetByIdAsync(id);
            if (transaction == null) return null;

            var dto = MapToResponse(transaction);

            string productName = await _productServiceClient.GetProductNameAsync(
                                    bearerToken, transaction.ProductId);
            dto.ProductName = productName;

            return dto;
        }

        public async Task<PaginatedResponse<TransactionHistoryResponseDto>> GetTransactionsByUserIdAsync(int userId, int page, int sizePerPage, string bearerToken)
        {
            var transactions = await _transactionHistoryRepository
               .GetByUserIdAsync(userId, page, sizePerPage);

            var totalCount = await _transactionHistoryRepository
                .GetTotalCountByUserIdAsync(userId);

            var productIds = transactions.Select(t => t.ProductId).Distinct().ToList();

            var productNames = new Dictionary<int, string>();
            foreach (var pid in productIds)
            {
                var name = await _productServiceClient.GetProductNameAsync(bearerToken, pid);
                productNames[pid] = name;
            }

            var dtoList = transactions.Select(t =>
            {
                var dto = MapToResponse(t);
                dto.ProductName = productNames[t.ProductId];
                return dto;
            }).ToList();

            var totalPage = (int)Math.Ceiling(totalCount / (double)sizePerPage);

            return new PaginatedResponse<TransactionHistoryResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = dtoList
            };
        }

        // Helper method
        private TransactionHistoryResponseDto MapToResponse(TransactionHistory transaction)
        {
            return new TransactionHistoryResponseDto
            {
                Id = transaction.Id,
                ProductId = transaction.ProductId,
                UserId = transaction.UserId,
                Quantity = transaction.Quantity,
                Price = transaction.Price,
                TotalPrice = transaction.TotalPrice,
                StatusName = transaction.Status?.StatusName ?? "Unknown",
                Remarks = transaction.Remarks,
                TransactionAt = transaction.TransactionAt,
                ModifiedOn = transaction.ModifiedOn
            };
        }
    }
}
