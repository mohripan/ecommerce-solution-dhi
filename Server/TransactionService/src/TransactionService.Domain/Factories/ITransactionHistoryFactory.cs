using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Factories
{
    public interface ITransactionHistoryFactory
    {
        TransactionHistory CreateTransaction(int productId, int userId, int quantity, double price, string? remarks = null);
    }
}
