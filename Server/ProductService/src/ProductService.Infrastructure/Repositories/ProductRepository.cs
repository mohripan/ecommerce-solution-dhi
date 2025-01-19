using Microsoft.EntityFrameworkCore;
using ProductService.Contracts.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync(int userId, int page, int sizePerPage, int? categoryId)
        {
            var query = _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * sizePerPage)
                .Take(sizePerPage)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetAllWithoutUserFilterAsync(int page, int sizePerPage, string? productName)
        {
            var query = _dbContext.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                productName = productName.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(productName));
            }

            return await query
                .OrderBy(p => p.Id)
                .Skip((page - 1) * sizePerPage)
                .Take(sizePerPage)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountWithoutUserFilterAsync(string? productName)
        {
            var query = _dbContext.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(productName))
            {
                productName = productName.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(productName));
            }

            return await query.CountAsync();
        }



        public async Task<int> GetTotalCountAsync(int userId, int? categoryId)
        {
            var query = _dbContext.Products
                .Where(p => p.UserId == userId)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return await query.CountAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);
        }

        public void Delete(Product product)
        {
            _dbContext.Products.Remove(product);
        }
    }
}
