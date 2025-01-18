using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Entities
{
    public class MstrRole
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = "SYS";
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
