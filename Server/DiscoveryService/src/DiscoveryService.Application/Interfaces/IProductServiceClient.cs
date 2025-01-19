using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Interfaces
{
    public interface IProductServiceClient
    {
        Task<(HttpStatusCode StatusCode, string Content)> CreateProductAsync(object productRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> UpdateProductAsync(int id, object productRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> ReStockProductAsync(int id, object restockRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GoodPurchasedAsync(int id, object purchaseRequestDto, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> DeleteProductAsync(int id, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GetProductsAsync(int page, int sizePerPage, int? categoryId, string authorizationHeader);
        Task<(HttpStatusCode StatusCode, string Content)> GetProductByIdAsync(int id);
        Task<(HttpStatusCode StatusCode, string Content)> SearchProductsAsync(string? productName, int page, int sizePerPage);
    }
}
