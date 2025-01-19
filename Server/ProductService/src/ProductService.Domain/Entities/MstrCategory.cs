using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Entities
{
    public class MstrCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = new DateTime(2025, 1, 1);
        public string CreatedBy { get; set; } = "SYS";
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
