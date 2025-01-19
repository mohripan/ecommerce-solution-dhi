using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; } = default!;
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

        public MstrCategory? Category { get; set; }

        private Product() { }

        public Product(int categoryId, int userId, double price, int createdBy)
        {
            CategoryId = categoryId;
            UserId = userId;
            Price = price;
            CreatedBy = createdBy;
            CreatedOn = DateTime.UtcNow;
            Quantity = 0;
        }
    }
}
