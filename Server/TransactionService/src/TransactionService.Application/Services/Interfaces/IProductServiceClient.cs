using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Application.Services.Interfaces
{
    public interface IProductServiceClient
    {
        Task<(int totalItems, List<(int productId, string productName)>)>
            GetSellerProductsAsync(string bearerToken, int page, int sizePerPage);
        Task<int> GetProductQuantityAsync(string bearerToken, int productId);
        Task<bool> MarkProductAsPurchasedAsync(string bearerToken, int productId, int purchasedQuantity);
        Task<string> GetProductNameAsync(string bearerToken, int productId);
    }
}
