using ProductService.Application.DTOs.Requests;
using ProductService.Application.DTOs.Responses;
using ProductService.Application.Services.Interfaces;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Contracts.Interfaces;
using ProductService.Domain.Factories;

namespace ProductService.Application.Services.Impls
{
    public class ProductAppService : IProductAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductFactory _productFactory;

        public ProductAppService(IProductRepository productRepository, IUnitOfWork unitOfWork, IProductFactory productFactory)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _productFactory = productFactory;
        }

        public async Task<ProductResponseDto> CreateProductAsync(ProductRequestDto requestDto, int userId)
        {
            var product = _productFactory.CreateProduct(requestDto.CategoryId, userId, requestDto.Price, userId);

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, ProductRequestDto requestDto, int userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.UserId != userId) return null;

            product.CategoryId = requestDto.CategoryId;
            product.Price = requestDto.Price;
            product.ModifiedBy = userId.ToString();
            product.ModifiedOn = DateTime.UtcNow;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<ProductResponseDto?> ReStockProductAsync(int id, ReStockRequestDto requestDto, int userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.UserId != userId) return null;

            product.Quantity += requestDto.Quantity;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<ProductResponseDto?> GoodPurchasedAsync(int id, ReStockRequestDto requestDto, int userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.UserId != userId || product.Quantity < requestDto.Quantity) return null;

            product.Quantity -= requestDto.Quantity;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(product);
        }

        public async Task<bool> DeleteProductAsync(int id, int userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.UserId != userId) return false;

            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResponse<ProductResponseDto>> GetProductsAsync(int userId, int page, int sizePerPage, int? categoryId)
        {
            var products = await _productRepository.GetAllAsync(userId, page, sizePerPage, categoryId);
            var totalCount = await _productRepository.GetTotalCountAsync(userId, categoryId);

            var totalPage = (int)Math.Ceiling((double)totalCount / sizePerPage);

            return new PaginatedResponse<ProductResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = products.Select(MapToResponse).ToList()
            };
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id, int userId)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.UserId != userId) return null;

            return MapToResponse(product);
        }

        private static ProductResponseDto MapToResponse(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                CategoryName = product.Category?.CategoryName ?? "Unknown",
                Quantity = product.Quantity,
                Price = product.Price,
                CreatedOn = product.CreatedOn
            };
        }
    }
}
