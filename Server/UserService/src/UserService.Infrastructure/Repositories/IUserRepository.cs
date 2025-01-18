using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<MstrUser?> GetByIdAsync(int id);
        Task<IReadOnlyList<MstrUser>> GetAllAsync();
        Task AddAsync(MstrUser user);
        void Update(MstrUser user);
        void Delete(MstrUser user);
    }
}
