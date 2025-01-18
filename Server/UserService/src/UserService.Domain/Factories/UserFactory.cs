using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Domain.Services;

namespace UserService.Domain.Factories
{
    public class UserFactory : IUserFactory
    {
        private readonly IUserDomainService _userDomainService;

        public UserFactory(IUserDomainService userDomainService)
        {
            _userDomainService = userDomainService;
        }

        public MstrUser CreateUser(string username, string email, int roleId)
        {
            var user = new MstrUser(username, email, roleId);
            _userDomainService.ValidateBeforeCreation(user);
            return user;
        }
    }
}
