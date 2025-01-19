using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Services;

namespace TransactionService.Domain.Factories
{
    public class TransactionHistoryFactory : ITransactionHistoryFactory
    {
        private readonly ITransactionHistoryDomainService _transactionDomainService;

        public TransactionHistoryFactory(ITransactionHistoryDomainService transactionDomainService)
        {
            _transactionDomainService = transactionDomainService;
        }

        public TransactionHistory CreateTransaction(int productId, int userId, int quantity, double price, string? remarks = null)
        {
            var transaction = new TransactionHistory(productId, userId, quantity, price, remarks);
            _transactionDomainService.ValidateTransaction(transaction);
            return transaction;
        }
    }
}
