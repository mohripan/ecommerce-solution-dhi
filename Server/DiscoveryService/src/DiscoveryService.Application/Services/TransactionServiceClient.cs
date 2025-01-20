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
    public class TransactionServiceClient : ITransactionServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _transactionServiceBaseUrl;

        public TransactionServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _transactionServiceBaseUrl = configuration["ServiceUrls:TransactionService"]!;
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> CreateTransactionAsync(object transactionRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(transactionRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/TransactionHistory", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> UpdateTransactionStatusAsync(int id, object updateRequestDto, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(updateRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PatchAsync($"api/TransactionHistory/{id}/status", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetTransactionByIdAsync(int id, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var response = await client.GetAsync($"api/TransactionHistory/{id}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetUserTransactionsAsync(int page, int sizePerPage, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var query = $"?page={page}&sizePerPage={sizePerPage}";
            var response = await client.GetAsync($"api/TransactionHistory{query}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetSellerProductTransactionsAsync(int page, int sizePerPage, string authorizationHeader)
        {
            var client = CreateClientWithAuthorization(authorizationHeader);
            var query = $"?page={page}&sizePerPage={sizePerPage}";
            var response = await client.GetAsync($"api/TransactionHistory/seller/product-transactions{query}");
            return await ExtractResponseAsync(response);
        }

        private HttpClient CreateClientWithAuthorization(string authorizationHeader)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            client.BaseAddress = new Uri(_transactionServiceBaseUrl);
            return client;
        }

        private static async Task<(HttpStatusCode StatusCode, string Content)> ExtractResponseAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, content);
        }
    }
}
