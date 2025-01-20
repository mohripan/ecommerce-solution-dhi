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
            var product = _productFactory.CreateProduct(requestDto.CategoryId, userId, requestDto.Price, userId, requestDto.Name);

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return await MapToResponseAsync(product);
        }

        public async Task<ProductResponseDto?> UpdateProductAsync(int id, ProductRequestDto requestDto, int userId)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;
            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("This product does not belong to you.");

            existing.Name = requestDto.Name;
            existing.CategoryId = requestDto.CategoryId;
            existing.Price = requestDto.Price;

            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = userId.ToString();

            _productRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            existing = await _productRepository.GetByIdAsync(id);

            return await MapToResponseAsync(existing);
        }

        public async Task<ProductResponseDto?> ReStockProductAsync(int id, ReStockRequestDto requestDto, int userId)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;
            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("This product does not belong to you.");

            existing.Quantity += requestDto.Quantity;
            if (existing.Quantity < 0) existing.Quantity = 0;

            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = userId.ToString();

            _productRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return await MapToResponseAsync(existing);
        }

        public async Task<ProductResponseDto?> GoodPurchasedAsync(int id, ReStockRequestDto requestDto, int userId)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;

            if (requestDto.Quantity <= 0)
                throw new ArgumentException("QuantityChange must be > 0 when purchasing.");

            if (existing.Quantity < requestDto.Quantity)
                throw new InvalidOperationException("Not enough stock to fulfill purchase.");

            existing.Quantity -= requestDto.Quantity;

            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedBy = userId.ToString();

            _productRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return await MapToResponseAsync(existing);
        }

        public async Task<bool> DeleteProductAsync(int id, int userId)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return false;
            if (existing.UserId != userId)
                throw new UnauthorizedAccessException("This product does not belong to you.");

            _productRepository.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResponse<ProductResponseDto>> GetProductsAsync(int userId, int page, int sizePerPage, int? categoryId)
        {
            var products = await _productRepository.GetAllAsync(userId, page, sizePerPage, categoryId);
            var totalCount = await _productRepository.GetTotalCountAsync(userId, categoryId);

            var totalPage = (int)Math.Ceiling((double)totalCount / sizePerPage);

            var productDtos = new List<ProductResponseDto>();
            foreach (var product in products)
            {
                var dto = await MapToResponseAsync(product);
                productDtos.Add(dto);
            }

            return new PaginatedResponse<ProductResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = productDtos
            };
        }

        public async Task<ProductResponseDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return await MapToResponseAsync(product);
        }

        public async Task<PaginatedResponse<ProductResponseDto>> SearchProductsAsync(string? productName, int page, int sizePerPage)
        {
            var allProducts = await _productRepository.GetAllWithoutUserFilterAsync(page, sizePerPage, productName);
            var totalCount = await _productRepository.GetTotalCountWithoutUserFilterAsync(productName);

            var totalPage = (int)Math.Ceiling((double)totalCount / sizePerPage);

            var productDtos = new List<ProductResponseDto>();
            foreach (var product in allProducts)
            {
                var dto = await MapToResponseAsync(product);
                productDtos.Add(dto);
            }

            return new PaginatedResponse<ProductResponseDto>
            {
                Search = new PaginationMetadata
                {
                    Total = totalCount,
                    TotalPage = totalPage,
                    SizePerPage = sizePerPage,
                    PageAt = page
                },
                Values = productDtos
            };
        }

        private async Task<ProductResponseDto> MapToResponseAsync(Product product)
        {
            var categoryName = product.Category?.CategoryName;

            if (string.IsNullOrEmpty(categoryName) && product.CategoryId > 0)
            {
                categoryName = await _productRepository.GetCategoryNameByIdAsync(product.CategoryId);
            }

            return new ProductResponseDto
            {
                Id = product.Id,
                ProductName = product.Name,
                CategoryName = categoryName ?? "Unknown",
                Quantity = product.Quantity,
                Price = product.Price,
                CreatedOn = product.CreatedOn
            };
        }
    }
}
