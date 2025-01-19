using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.Requests
{
    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public double Price { get; set; }
    }
}
