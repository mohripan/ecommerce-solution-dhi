using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain.Entities
{
    public class MstrUser
    {
        public int Id { get; set; }
        public string Username { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public string Password { get; private set; } = default!;
        public int RoleId { get; private set; }

        public MstrRole? Role { get; private set; }

        private MstrUser()
        {
            
        }

        public MstrUser(string username, string email, string passwordHash, int roleId)
        {
            Username = username;
            Email = email;
            Password = passwordHash;
            RoleId = roleId;
        }

        public void UpdateUser(string username, string email, string passwordHash, int roleId)
        {
            Username = username;
            Email = email;
            Password = passwordHash;
            RoleId = roleId;
        }
    }
}
