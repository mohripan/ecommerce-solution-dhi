using DiscoveryService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DiscoveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionServiceClient _transactionServiceClient;

        public TransactionsController(ITransactionServiceClient transactionServiceClient)
        {
            _transactionServiceClient = transactionServiceClient;
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Create([FromBody] object transactionRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized("Authorization token is missing.");

            var (statusCode, content) = await _transactionServiceClient.CreateTransactionAsync(transactionRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] object updateRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized("Authorization token is missing.");

            var (statusCode, content) = await _transactionServiceClient.UpdateTransactionStatusAsync(id, updateRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetById(int id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized("Authorization token is missing.");

            var (statusCode, content) = await _transactionServiceClient.GetTransactionByIdAsync(id, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpGet]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized("Authorization token is missing.");

            var (statusCode, content) = await _transactionServiceClient.GetUserTransactionsAsync(page, sizePerPage, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpGet("seller/product-transactions")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetSellerProductTransactions([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
                return Unauthorized("Authorization token is missing.");

            var (statusCode, content) = await _transactionServiceClient.GetSellerProductTransactionsAsync(page, sizePerPage, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        private IActionResult CreateResponse(HttpStatusCode statusCode, string content)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => Ok(JsonSerializer.Deserialize<object>(content)),
                HttpStatusCode.Created => Created("", JsonSerializer.Deserialize<object>(content)),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.Unauthorized => Unauthorized(content),
                HttpStatusCode.Forbidden => Forbid(content),
                HttpStatusCode.NotFound => NotFound(content),
                _ => StatusCode((int)statusCode, content)
            };
        }
    }
}
