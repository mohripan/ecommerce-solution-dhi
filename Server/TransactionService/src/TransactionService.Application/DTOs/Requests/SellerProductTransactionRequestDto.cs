using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs.Responses;

namespace TransactionService.Application.DTOs.Requests
{
    public class SellerProductTransactionRequestDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<TransactionHistoryResponseDto> TransactionHistory { get; set; } = new();
    }
}
