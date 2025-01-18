using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Contracts.Interfaces;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MstrUser?> GetByIdAsync(int id)
        {
            return await _dbContext.MstrUsers
                                   .Include(u => u.Role)
                                   .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IReadOnlyList<MstrUser>> GetAllAsync(int page, int sizePerPage, int? roleId)
        {
            var query = _dbContext.MstrUsers.Include(u => u.Role).AsQueryable();

            if (roleId.HasValue)
                query = query.Where(u => u.RoleId == roleId.Value);

            return await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * sizePerPage)
                .Take(sizePerPage)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(int? roleId)
        {
            var query = _dbContext.MstrUsers.AsQueryable();

            if (roleId.HasValue)
                query = query.Where(u => u.RoleId == roleId.Value);

            return await query.CountAsync();
        }

        public async Task<MstrUser?> GetByEmailAsync(string email)
        {
            return await _dbContext.MstrUsers
                           .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(MstrUser user)
        {
            await _dbContext.MstrUsers.AddAsync(user);
        }

        public void Update(MstrUser user)
        {
            _dbContext.MstrUsers.Update(user);
        }

        public void Delete(MstrUser user)
        {
            _dbContext.MstrUsers.Remove(user);
        }
    }
}
