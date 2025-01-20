using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TransactionService.Application.Services.Interfaces;

namespace TransactionService.Application.Services.Impls
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(int totalItems, List<(int productId, string productName)>)>
            GetSellerProductsAsync(string bearerToken, int page, int sizePerPage)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.GetAsync($"api/products?page={page}&sizePerPage={sizePerPage}");
            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(contentString);

            var searchElement = doc.RootElement.GetProperty("search");
            int totalItems = searchElement.GetProperty("total").GetInt32();

            var valuesElement = doc.RootElement.GetProperty("values");
            var products = new List<(int productId, string productName)>();
            foreach (var item in valuesElement.EnumerateArray())
            {
                var productId = item.GetProperty("id").GetInt32();
                string productName = item.GetProperty("categoryName").GetString() ?? "Unknown";

                products.Add((productId, productName));
            }

            return (totalItems, products);
        }

        public async Task<int> GetProductQuantityAsync(string bearerToken, int productId)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.GetAsync($"api/products/{productId}");
            response.EnsureSuccessStatusCode();

            var contentString = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(contentString);

            var quantity = doc.RootElement.GetProperty("quantity").GetInt32();
            return quantity;
        }

        public async Task<string> GetProductNameAsync(string bearerToken, int productId)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await _httpClient.GetAsync($"/api/products/{productId}");
            if (!response.IsSuccessStatusCode)
                return "Unknown Product";

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            if (!doc.RootElement.TryGetProperty("productName", out var nameProp))
                return "Unknown Product";

            return nameProp.GetString() ?? "Unknown Product";
        }


        public async Task<bool> MarkProductAsPurchasedAsync(string bearerToken, int productId, int purchasedQuantity)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);

            var content = new StringContent(
                JsonSerializer.Serialize(new { quantity = purchasedQuantity }),
                Encoding.UTF8,
                "application/json"
            );

            // /api/products/{id}/purchased
            var response = await _httpClient.PatchAsync($"api/products/{productId}/purchased", content);
            return response.IsSuccessStatusCode;
        }
    }

}
