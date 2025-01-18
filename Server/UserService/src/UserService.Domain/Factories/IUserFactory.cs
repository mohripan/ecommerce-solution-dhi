using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Domain.Factories
{
    public interface IUserFactory
    {
        MstrUser CreateUser(string username, string email, int roleId);
    }
}
