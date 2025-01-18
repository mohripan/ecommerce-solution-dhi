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

        public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString("N");

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = "SYS";
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedBy { get; set; }

        private MstrUser()
        {
        }

        public MstrUser(string username, string email, string passwordHash, int roleId)
        {
            Username = username;
            Email = email;
            Password = passwordHash;
            RoleId = roleId;
            CreatedOn = DateTime.UtcNow;
            CreatedBy = "SYS";
        }

        public void UpdateUser(string username, string email, string passwordHash, int roleId)
        {
            Username = username;
            Email = email;
            Password = passwordHash;
            RoleId = roleId;
            ModifiedOn = DateTime.UtcNow;
            ModifiedBy = "SYS";
        }
        public void RefreshSecurityStamp()
        {
            SecurityStamp = Guid.NewGuid().ToString("N");
        }

    }
}
