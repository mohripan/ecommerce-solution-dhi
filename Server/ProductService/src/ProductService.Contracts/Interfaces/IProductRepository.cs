using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Entities;

namespace ProductService.Contracts.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetAllAsync(int userId, int page, int sizePerPage, int? categoryId);
        Task<IReadOnlyList<Product>> GetAllWithoutUserFilterAsync(int page, int sizePerPage, string? productName);
        Task<int> GetTotalCountAsync(int userId, int? categoryId);
        Task<int> GetTotalCountWithoutUserFilterAsync(string? productName);
        Task<string?> GetCategoryNameByIdAsync(int categoryId);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
