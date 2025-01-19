using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Services
{
    public class ProductDomainService : IProductDomainService
    {
        public void ValidateBeforeCreation(Product product)
        {
            if (product.Price < 0)
                throw new ArgumentException("Price cannot be lower than 0");
        }
    }
}
