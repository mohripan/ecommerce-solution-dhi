using DiscoveryService.Application.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private UserServiceClient _userServiceClient;

        [TestInitialize]
        public void Setup()
        {
            var fakeHandler = new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"message\":\"Success\"}");
            var fakeHttpClientFactory = new FakeHttpClientFactory(fakeHandler);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "UserService:BaseUrl", "http://localhost:8080" }
                })
                .Build();

            _userServiceClient = new UserServiceClient(fakeHttpClientFactory, configuration);
        }

        [TestMethod]
        public async Task GetAllUsersAsync_ReturnsSuccess()
        {
            var (statusCode, content) = await _userServiceClient.GetAllUsersAsync(1, 10, null);

            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            StringAssert.Contains(content, "Success");
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsSuccess()
        {
            var (statusCode, content) = await _userServiceClient.GetUserByIdAsync(1);

            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            StringAssert.Contains(content, "Success");
        }

        [TestMethod]
        public async Task LoginAsync_ReturnsSuccess()
        {
            var loginDto = new { Email = "test@example.com", Password = "password123" };

            var (statusCode, content) = await _userServiceClient.LoginAsync(loginDto);

            Assert.AreEqual(HttpStatusCode.OK, statusCode);
            StringAssert.Contains(content, "Success");
        }

        [TestMethod]
        public async Task CreateUserAsync_ReturnsCreated()
        {
            var userDto = new { Username = "John", Email = "john@example.com", Password = "password123" };

            var (statusCode, content) = await _userServiceClient.CreateUserAsync(userDto);

            Assert.AreEqual(HttpStatusCode.Created, statusCode);
            StringAssert.Contains(content, "Success");
        }
    }

    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_responseContent)
            });
        }
    }

    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;

        public FakeHttpClientFactory(HttpMessageHandler handler)
        {
            _handler = handler;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(_handler);
        }
    }
}
