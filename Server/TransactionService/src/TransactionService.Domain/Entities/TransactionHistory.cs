using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Domain.Entities
{
    public class TransactionHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public DateTime TransactionAt { get; set; }
        public int StatusId { get; set; }
        public MstrStatus? Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ModifiedOn { get; set; }

        private TransactionHistory() { }

        public TransactionHistory(int productId, int userId, int quantity, double price, string? remarks = null)
        {
            if (productId <= 0) throw new ArgumentException("ProductId must be greater than 0.", nameof(productId));
            if (userId <= 0) throw new ArgumentException("UserId must be greater than 0.", nameof(userId));
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));
            if (price <= 0) throw new ArgumentException("Price must be greater than 0.", nameof(price));

            ProductId = productId;
            UserId = userId;
            Quantity = quantity;
            Price = price;
            TotalPrice = quantity * price;
            TransactionAt = DateTime.UtcNow;
            StatusId = 1;
            Remarks = remarks;
            ModifiedOn = null;
        }
    }
}
