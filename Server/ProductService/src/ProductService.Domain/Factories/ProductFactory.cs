using ProductService.Domain.Entities;
using ProductService.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Factories
{
    public class ProductFactory : IProductFactory
    {
        private readonly IProductDomainService _productDomainService;

        public ProductFactory(IProductDomainService productDomainService)
        {
            _productDomainService = productDomainService;
        }

        public Product CreateProduct(int categoryId, int userId, double price, int createdBy)
        {
            var product = new Product(categoryId, userId, price, createdBy);
            _productDomainService.ValidateBeforeCreation(product);
            return product;
        }
    }
}
