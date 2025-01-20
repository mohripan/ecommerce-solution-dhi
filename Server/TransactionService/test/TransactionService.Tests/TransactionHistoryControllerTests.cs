using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Api.Controllers;
using TransactionService.Application.DTOs.Requests;
using TransactionService.Application.DTOs.Responses;
using TransactionService.Application.Services.Interfaces;

namespace TransactionService.Tests
{
    [TestClass]
    public class TransactionHistoryControllerTests
    {
        private Mock<ITransactionHistoryAppService> _transactionHistoryAppServiceMock;
        private TransactionHistoryController _controller;

        [TestInitialize]
        public void Setup()
        {
            _transactionHistoryAppServiceMock = new Mock<ITransactionHistoryAppService>();
            _controller = new TransactionHistoryController(_transactionHistoryAppServiceMock.Object);
        }

        [TestMethod]
        public async Task Create_ValidRequest_ReturnsCreatedResult()
        {
            // Arrange
            var request = new TransactionHistoryRequestDto { ProductId = 1, Quantity = 2, Price = 50.0 };
            var userId = "1";
            var responseDto = new TransactionHistoryResponseDto { Id = 1, UserId = 1, ProductName = "Test Product", TotalPrice = 100.0 };

            _transactionHistoryAppServiceMock
                .Setup(s => s.CreateTransactionAsync(request, It.IsAny<int>()))
                .ReturnsAsync(responseDto);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.Create(request) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.GetById), result.ActionName);
            Assert.AreEqual(responseDto, result.Value);
        }

        [TestMethod]
        public async Task Create_MissingUserId_ReturnsUnauthorized()
        {
            // Arrange
            var request = new TransactionHistoryRequestDto { ProductId = 1, Quantity = 2, Price = 50.0 };
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            // Act
            var result = await _controller.Create(request) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.AreEqual("User ID not found in token.", result.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var id = 1;
            var request = new UpdateTransactionStatusRequestDto { StatusId = 2 };
            var bearerToken = "BearerToken";
            var responseDto = new TransactionHistoryResponseDto { Id = 1, UserId = 1, ProductName = "Test Product", StatusName = "Completed" };

            _transactionHistoryAppServiceMock
                .Setup(s => s.UpdateTransactionStatusAsync(id, request.StatusId, bearerToken))
                .ReturnsAsync(responseDto);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }));
            httpContext.Request.Headers["Authorization"] = "Bearer " + bearerToken;
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.UpdateStatus(id, request) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseDto, result.Value);
        }

        [TestMethod]
        public async Task UpdateStatus_MissingBearerToken_ReturnsUnauthorized()
        {
            // Arrange
            var id = 1;
            var request = new UpdateTransactionStatusRequestDto { StatusId = 2 };

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.UpdateStatus(id, request) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.AreEqual("Missing Bearer token in Authorization header.", result.Value);
        }

        [TestMethod]
        public async Task GetById_ValidId_ReturnsOkResult()
        {
            // Arrange
            var id = 1;
            var userId = "1";
            var responseDto = new TransactionHistoryResponseDto { Id = id, UserId = 1, ProductName = "Test Product" };

            _transactionHistoryAppServiceMock
                .Setup(s => s.GetTransactionByIdAsync(id, It.IsAny<string>()))
                .ReturnsAsync(responseDto);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            httpContext.Request.Headers["Authorization"] = "Bearer BearerToken";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.GetById(id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(responseDto, result.Value);
        }

        [TestMethod]
        public async Task GetById_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = 1;
            var userId = "1";

            _transactionHistoryAppServiceMock
                .Setup(s => s.GetTransactionByIdAsync(id, It.IsAny<string>()))
                .ReturnsAsync((TransactionHistoryResponseDto)null);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            httpContext.Request.Headers["Authorization"] = "Bearer BearerToken";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.GetById(id) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetAll_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var userId = "1";
            var page = 1;
            var sizePerPage = 10;
            var response = new PaginatedResponse<TransactionHistoryResponseDto>
            {
                Search = new PaginationMetadata { Total = 2, TotalPage = 1, SizePerPage = 10, PageAt = 1 },
                Values = new List<TransactionHistoryResponseDto>
                {
                    new TransactionHistoryResponseDto { Id = 1, ProductName = "Test Product 1" },
                    new TransactionHistoryResponseDto { Id = 2, ProductName = "Test Product 2" }
                }
            };

            _transactionHistoryAppServiceMock
                .Setup(s => s.GetTransactionsByUserIdAsync(It.IsAny<int>(), page, sizePerPage, It.IsAny<string>()))
                .ReturnsAsync(response);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }));
            httpContext.Request.Headers["Authorization"] = "Bearer BearerToken";
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.GetAll(page, sizePerPage) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(response, result.Value);
        }
    }
}
