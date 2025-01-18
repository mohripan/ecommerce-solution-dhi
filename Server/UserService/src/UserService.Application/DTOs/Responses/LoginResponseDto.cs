using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.DTOs.Responses
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
