using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Contracts.Interfaces
{
    public interface IUserRepository
    {
        Task<MstrUser?> GetByIdAsync(int id);
        Task<IReadOnlyList<MstrUser>> GetAllAsync(int page, int sizePerPage, int? roleId);
        Task<int> GetTotalCountAsync(int? roleId);
        Task<MstrUser?> GetByEmailAsync(string email);
        Task AddAsync(MstrUser user);
        void Update(MstrUser user);
        void Delete(MstrUser user);
    }
}
