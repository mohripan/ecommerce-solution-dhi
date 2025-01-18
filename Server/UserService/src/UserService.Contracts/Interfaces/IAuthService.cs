using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Contracts.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateUserAsync(string email, string password);
        Task<string> GenerateJwtTokenAsync(int userId);
    }
}
