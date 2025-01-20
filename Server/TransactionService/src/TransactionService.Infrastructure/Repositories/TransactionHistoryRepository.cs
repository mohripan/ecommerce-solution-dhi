using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Contracts.Interfaces;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Data;

namespace TransactionService.Infrastructure.Repositories
{
    public class TransactionHistoryRepository : ITransactionHistoryRepository
    {
        private readonly TransactionDbContext _dbContext;

        public TransactionHistoryRepository(TransactionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TransactionHistory?> GetByIdAsync(int id)
        {
            return await _dbContext.TransactionHistories
                                   .Include(th => th.Status)
                                   .FirstOrDefaultAsync(th => th.Id == id);
        }

        public async Task<IReadOnlyList<TransactionHistory>> GetByUserIdAsync(int userId, int page, int sizePerPage)
        {
            return await _dbContext.TransactionHistories
                                   .Include(th => th.Status)
                                   .Where(th => th.UserId == userId)
                                   .OrderBy(th => th.TransactionAt)
                                   .Skip((page - 1) * sizePerPage)
                                   .Take(sizePerPage)
                                   .ToListAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _dbContext.TransactionHistories
                                   .Where(th => th.UserId == userId)
                                   .CountAsync();
        }

        public async Task AddAsync(TransactionHistory transactionHistory)
        {
            await _dbContext.TransactionHistories.AddAsync(transactionHistory);
        }

        public async Task UpdateStatusAsync(int transactionId, int statusId)
        {
            var transaction = await _dbContext.TransactionHistories.FirstOrDefaultAsync(th => th.Id == transactionId);
            if (transaction == null)
                throw new ArgumentException("Transaction not found.");

            transaction.StatusId = statusId;
            transaction.ModifiedOn = DateTime.UtcNow;

            _dbContext.TransactionHistories.Update(transaction);
        }

        public async Task<IReadOnlyList<TransactionHistory>> GetByProductIdsAsync(List<int> productIds)
        {
            return await _dbContext.TransactionHistories
                .Include(th => th.Status)
                .Where(th => productIds.Contains(th.ProductId))
                .ToListAsync();
        }
    }
}
