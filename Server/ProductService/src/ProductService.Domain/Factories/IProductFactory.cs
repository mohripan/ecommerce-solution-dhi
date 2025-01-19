using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Factories
{
    public interface IProductFactory
    {
        Product CreateProduct(int categoryId, int userId, double price, int createdBy, string name);
    }
}
