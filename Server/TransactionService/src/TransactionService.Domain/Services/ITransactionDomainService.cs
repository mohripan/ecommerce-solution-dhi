using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Services
{
    public interface ITransactionDomainService
    {
        void ValidateTransaction(Transaction transaction);
    }
}
