using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Contracts.Interfaces
{
    public interface ITransactionHistoryRepository
    {
        Task<TransactionHistory?> GetByIdAsync(int id);
        Task<IReadOnlyList<TransactionHistory>> GetByUserIdAsync(int userId, int page, int sizePerPage);
        Task<int> GetTotalCountByUserIdAsync(int userId);
        Task AddAsync(TransactionHistory transactionHistory);
        Task UpdateStatusAsync(int transactionId, int statusId);
    }
}
