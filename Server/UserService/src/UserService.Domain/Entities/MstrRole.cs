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
    }
}
