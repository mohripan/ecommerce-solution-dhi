using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Domain.Entities
{
    public class TransactionHistory
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public int UserId { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public double TotalPrice { get; private set; }
        public DateTime TransactionAt { get; private set; }
        public int StatusId { get; private set; }
        public MstrStatus? Status { get; private set; }
        public string? Remarks { get; private set; }

        private TransactionHistory() { }

        public TransactionHistory(int productId, int userId, int quantity, double price, int statusId, string? remarks = null)
        {
            if (productId <= 0) throw new ArgumentException("ProductId must be greater than 0.", nameof(productId));
            if (userId <= 0) throw new ArgumentException("UserId must be greater than 0.", nameof(userId));
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));
            if (price <= 0) throw new ArgumentException("Price must be greater than 0.", nameof(price));
            if (statusId <= 0) throw new ArgumentException("StatusId must be greater than 0.", nameof(statusId));

            ProductId = productId;
            UserId = userId;
            Quantity = quantity;
            Price = price;
            TotalPrice = quantity * price;
            TransactionAt = DateTime.UtcNow;
            StatusId = statusId;
            Remarks = remarks;
        }
    }
}
