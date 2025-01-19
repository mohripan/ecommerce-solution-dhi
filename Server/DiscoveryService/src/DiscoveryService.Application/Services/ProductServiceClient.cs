using DiscoveryService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _productServiceBaseUrl;

        public ProductServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _productServiceBaseUrl = configuration["ServiceUrls:ProductService"]!;
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> CreateProductAsync(object productRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(productRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/products", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> UpdateProductAsync(int id, object productRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(productRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/products/{id}", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> ReStockProductAsync(int id, object restockRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(restockRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"api/products/{id}/restock", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GoodPurchasedAsync(int id, object purchaseRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(purchaseRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"api/products/{id}/purchased", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> DeleteProductAsync(int id, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var response = await client.DeleteAsync($"api/products/{id}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetProductsAsync(int page, int sizePerPage, int? categoryId, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var query = $"?page={page}&sizePerPage={sizePerPage}" + (categoryId.HasValue ? $"&categoryId={categoryId}" : "");
            var response = await client.GetAsync($"api/products{query}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetProductByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_productServiceBaseUrl}/api/products/{id}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> SearchProductsAsync(string? productName, int page, int sizePerPage)
        {
            var client = _httpClientFactory.CreateClient();
            var query = $"?productName={productName}&page={page}&sizePerPage={sizePerPage}";
            var response = await client.GetAsync($"{_productServiceBaseUrl}/api/products/search{query}");
            return await ExtractResponseAsync(response);
        }

        private HttpClient CreateClientWithAuthorization(string authorizationHeader)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            client.BaseAddress = new Uri(_productServiceBaseUrl);
            return client;
        }

        private static async Task<(HttpStatusCode StatusCode, string Content)> ExtractResponseAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, content);
        }
    }
}
