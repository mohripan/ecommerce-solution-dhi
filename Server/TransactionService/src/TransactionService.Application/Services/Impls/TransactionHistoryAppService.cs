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

        public TransactionHistoryAppService(
            ITransactionHistoryRepository transactionHistoryRepository,
            IUnitOfWork unitOfWork,
            ITransactionHistoryFactory transactionHistoryFactory)
        {
            _transactionHistoryRepository = transactionHistoryRepository;
            _unitOfWork = unitOfWork;
            _transactionHistoryFactory = transactionHistoryFactory;
        }

        public async Task<TransactionHistoryResponseDto> CreateTransactionAsync(TransactionHistoryRequestDto requestDto)
        {
            var transaction = _transactionHistoryFactory.CreateTransaction(
                requestDto.ProductId,
                requestDto.UserId,
                requestDto.Quantity,
                requestDto.Price,
                requestDto.Remarks ?? string.Empty
            );

            await _transactionHistoryRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transaction);
        }

        public async Task<TransactionHistoryResponseDto?> UpdateTransactionStatusAsync(int id, int statusId)
        {
            var transaction = await _transactionHistoryRepository.GetByIdAsync(id);
            if (transaction == null) throw new GlobalException("Transaction not found.");

            if (transaction.StatusId == 3 && statusId == 2) // Cancelled => Completed
                throw new GlobalException("Cannot change status from Cancelled to Completed.");
            if (transaction.StatusId == 2 && statusId == 3) // Completed => Cancelled
                throw new GlobalException("Cannot change status from Completed to Cancelled.");

            transaction.StatusId = statusId;
            transaction.ModifiedOn = DateTime.UtcNow;

            _transactionHistoryRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transaction);
        }

        public async Task<TransactionHistoryResponseDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionHistoryRepository.GetByIdAsync(id);
            return transaction == null ? null : MapToResponse(transaction);
        }

        public async Task<PaginatedResponse<TransactionHistoryResponseDto>> GetTransactionsByUserIdAsync(int userId, int page, int sizePerPage)
        {
            var transactions = await _transactionHistoryRepository.GetByUserIdAsync(userId, page, sizePerPage);
            var totalCount = await _transactionHistoryRepository.GetTotalCountByUserIdAsync(userId);

            var totalPage = (int)Math.Ceiling((double)totalCount / sizePerPage);

            return new PaginatedResponse<TransactionHistoryResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = transactions.Select(MapToResponse).ToList()
            };
        }

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
