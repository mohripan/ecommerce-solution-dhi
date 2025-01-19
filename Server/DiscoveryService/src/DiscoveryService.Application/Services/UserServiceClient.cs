using DiscoveryService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Services
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _userServiceBaseUrl;

        public UserServiceClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _userServiceBaseUrl = configuration["UserService:BaseUrl"]!;
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetAllUsersAsync(int page, int sizePerPage, int? roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var query = $"?page={page}&sizePerPage={sizePerPage}" + (roleId.HasValue ? $"&roleId={roleId}" : "");
            var response = await client.GetAsync($"{_userServiceBaseUrl}/api/users{query}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> GetUserByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"{_userServiceBaseUrl}/api/users/{id}");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> LoginAsync(object loginRequestDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonSerializer.Serialize(loginRequestDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_userServiceBaseUrl}/api/users/login", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> CreateUserAsync(object userCreateDto)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonSerializer.Serialize(userCreateDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_userServiceBaseUrl}/api/users", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> UpdateUserAsync(object userUpdateDto, string authorizationHeader)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            var jsonContent = new StringContent(JsonSerializer.Serialize(userUpdateDto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_userServiceBaseUrl}/api/users", jsonContent);
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> DeleteUserAsync(string authorizationHeader)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            var response = await client.DeleteAsync($"{_userServiceBaseUrl}/api/users");
            return await ExtractResponseAsync(response);
        }

        public async Task<(HttpStatusCode StatusCode, string Content)> LogoutUserAsync(string authorizationHeader)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            var response = await client.PostAsync($"{_userServiceBaseUrl}/api/users/logout", null);
            return await ExtractResponseAsync(response);
        }

        private static async Task<(HttpStatusCode StatusCode, string Content)> ExtractResponseAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return (response.StatusCode, content);
        }
    }
}
