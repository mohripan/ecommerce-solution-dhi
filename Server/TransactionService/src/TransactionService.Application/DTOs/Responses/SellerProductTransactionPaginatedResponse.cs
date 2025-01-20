using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs.Requests;

namespace TransactionService.Application.DTOs.Responses
{
    public class SellerProductTransactionPaginatedResponse
    {
        public PaginationMetadata Search { get; set; } = default!;
        public IReadOnlyList<SellerProductTransactionRequestDto> Values { get; set; }
            = new List<SellerProductTransactionRequestDto>();
    }
}
