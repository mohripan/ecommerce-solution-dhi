using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Services
{
    public class TransactionDomainHistoryService : ITransactionHistoryDomainService
    {
        public void ValidateTransaction(TransactionHistory transaction)
        {
            if (transaction == null)
                throw new ArgumentException("Transaction cannot be null.", nameof(transaction));

            if (transaction.TotalPrice <= 0)
                throw new ArgumentException("TotalPrice must be greater than 0.");

            if (transaction.StatusId <= 0)
                throw new ArgumentException("StatusId must be valid.");
        }
    }
}
