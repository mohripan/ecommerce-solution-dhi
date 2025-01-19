using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.Responses
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = default!;
        public int Quantity { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
