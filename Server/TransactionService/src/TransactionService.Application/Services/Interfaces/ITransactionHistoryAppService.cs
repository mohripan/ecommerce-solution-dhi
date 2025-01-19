using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Application.Services.Interfaces
{
    public interface ITransactionHistoryAppService
    {
        Task<TransactionHistoryResponseDto> CreateTransactionAsync(TransactionHistoryRequestDto requestDto);
        Task<TransactionHistoryResponseDto?> UpdateTransactionStatusAsync(int id, int statusId);
        Task<TransactionHistoryResponseDto?> GetTransactionByIdAsync(int id);
        Task<PaginatedResponse<TransactionHistoryResponseDto>> GetTransactionsByUserIdAsync(int userId, int page, int sizePerPage);
    }
}
