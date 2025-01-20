using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.DTOs.Requests;
using TransactionService.Application.DTOs.Responses;

namespace TransactionService.Application.Services.Interfaces
{
    public interface ITransactionHistoryAppService
    {
        Task<TransactionHistoryResponseDto> CreateTransactionAsync(TransactionHistoryRequestDto requestDto, int userId);
        Task<TransactionHistoryResponseDto?> UpdateTransactionStatusAsync(int id, int statusId, string bearerToken);
        Task<SellerProductTransactionPaginatedResponse> GetSellerProductTransactionsAsync(int sellerId, string bearerToken, int page, int sizePerPage);
        Task<TransactionHistoryResponseDto?> GetTransactionByIdAsync(
           int id,
           string bearerToken);
        Task<PaginatedResponse<TransactionHistoryResponseDto>> GetTransactionsByUserIdAsync(
            int userId,
            int page,
            int sizePerPage,
            string bearerToken);

    }
}
