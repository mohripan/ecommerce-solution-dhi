using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IReadOnlyList<MstrUser>> GetAllAsync()
        {
            return await _dbContext.MstrUsers
                                   .Include(u => u.Role)
                                   .ToListAsync();
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
