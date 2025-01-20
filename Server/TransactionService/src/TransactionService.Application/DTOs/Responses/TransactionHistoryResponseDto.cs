using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransactionService.Application.DTOs.Responses
{
    public class TransactionHistoryResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime TransactionAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? ModifiedOn { get; set; }
    }
}
