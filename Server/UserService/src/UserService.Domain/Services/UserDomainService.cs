using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Services
{
    
    public class UserDomainService : IUserDomainService
    {
        public void ValidateBeforeCreation(Entities.MstrUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email cannot be empty");

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Password cannot be empty");
        }
    }
}
