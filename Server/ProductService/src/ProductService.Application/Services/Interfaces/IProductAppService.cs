using ProductService.Application.DTOs.Requests;
using ProductService.Application.DTOs.Responses;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Services.Interfaces
{
    public interface IProductAppService
    {
        Task<ProductResponseDto> CreateProductAsync(ProductRequestDto requestDto, int userId);
        Task<ProductResponseDto?> UpdateProductAsync(int id, ProductRequestDto requestDto, int userId);
        Task<bool> DeleteProductAsync(int id, int userId);
        Task<PaginatedResponse<ProductResponseDto>> GetProductsAsync(int userId, int page, int sizePerPage, int? categoryId);
        Task<ProductResponseDto?> GetProductByIdAsync(int id);
        Task<ProductResponseDto?> ReStockProductAsync(int id, ReStockRequestDto requestDto, int userId);
        Task<ProductResponseDto?> GoodPurchasedAsync(int id, ReStockRequestDto requestDto, int userId);
        Task<PaginatedResponse<ProductResponseDto>> SearchProductsAsync(string? productName, int page, int sizePerPage);
    }
}
