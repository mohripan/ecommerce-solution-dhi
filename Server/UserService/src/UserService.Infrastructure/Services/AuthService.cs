using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Contracts.Interfaces;

namespace UserService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        public Task<string> GenerateJwtTokenAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateUserAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
