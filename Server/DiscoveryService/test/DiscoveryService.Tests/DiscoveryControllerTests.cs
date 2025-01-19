using DiscoveryService.Api.Controllers;
using DiscoveryService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Tests
{
    [TestClass]
    public class DiscoveryControllerTests
    {
        private DiscoveryController _controller;

        [TestInitialize]
        public void Setup()
        {
            var fakeServiceClient = new FakeUserServiceClient();
            _controller = new DiscoveryController(fakeServiceClient);
        }

        [TestMethod]
        public async Task GetAllUsers_ReturnsOk()
        {
            var result = await _controller.GetAllUsers(1, 10, null) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "John");
        }

        [TestMethod]
        public async Task GetUserById_ReturnsOk()
        {
            var result = await _controller.GetUserById(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "John");
        }

        [TestMethod]
        public async Task Login_ReturnsOk()
        {
            var loginDto = new { Email = "john@example.com", Password = "password123" };

            var result = await _controller.Login(loginDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "abc123");
        }

        [TestMethod]
        public async Task CreateUser_ReturnsCreated()
        {
            var userDto = new { Username = "John", Email = "john@example.com", Password = "password123" };

            var result = await _controller.CreateUser(userDto) as CreatedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "John");
        }

        [TestMethod]
        public async Task UpdateUser_ReturnsOk()
        {
            var userUpdateDto = new { Username = "UpdatedJohn" };
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer token";

            var result = await _controller.UpdateUser(userUpdateDto) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "UpdatedJohn");
        }

        [TestMethod]
        public async Task DeleteUser_ReturnsNoContent()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer token";

            var result = await _controller.DeleteUser() as NoContentResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public async Task Logout_ReturnsOk()
        {
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer token";

            var result = await _controller.Logout() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            StringAssert.Contains(result.Value.ToString(), "Logout successful");
        }
    }

    public class FakeUserServiceClient : IUserServiceClient
    {
        public Task<(HttpStatusCode StatusCode, string Content)> GetAllUsersAsync(int page, int sizePerPage, int? roleId)
        {
            return Task.FromResult((HttpStatusCode.OK, "[{\"id\":1,\"username\":\"John\"}]"));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> GetUserByIdAsync(int id)
        {
            return Task.FromResult((HttpStatusCode.OK, "{\"id\":1,\"username\":\"John\"}"));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> LoginAsync(object loginRequestDto)
        {
            return Task.FromResult((HttpStatusCode.OK, "{\"token\":\"abc123\"}"));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> CreateUserAsync(object userCreateDto)
        {
            return Task.FromResult((HttpStatusCode.Created, "{\"id\":1,\"username\":\"John\"}"));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> UpdateUserAsync(object userUpdateDto, string authorizationHeader)
        {
            return Task.FromResult((HttpStatusCode.OK, "{\"id\":1,\"username\":\"UpdatedJohn\"}"));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> DeleteUserAsync(string authorizationHeader)
        {
            return Task.FromResult((HttpStatusCode.NoContent, string.Empty));
        }

        public Task<(HttpStatusCode StatusCode, string Content)> LogoutUserAsync(string authorizationHeader)
        {
            return Task.FromResult((HttpStatusCode.OK, "{\"message\":\"Logout successful\"}"));
        }
    }
}
