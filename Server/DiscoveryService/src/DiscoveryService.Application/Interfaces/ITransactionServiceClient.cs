using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Interfaces
{
    public interface ITransactionServiceClient
    {
        Task<(HttpStatusCode StatusCode, string Content)> CreateTransactionAsync(object transactionRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> UpdateTransactionStatusAsync(int id, object updateRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GetTransactionByIdAsync(int id, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GetUserTransactionsAsync(int page, int sizePerPage, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GetSellerProductTransactionsAsync(int page, int sizePerPage, string authorizationHeader);
    }
}
