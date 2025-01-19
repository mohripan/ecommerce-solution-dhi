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
        Task<int> GetTotalCountAsync(int userId, int? categoryId);
        Task AddAsync(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
