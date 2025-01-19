using DiscoveryService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DiscoveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServiceClient _productServiceClient;

        public ProductsController(IProductServiceClient productServiceClient)
        {
            _productServiceClient = productServiceClient;
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([FromBody] object productRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.CreateProductAsync(productRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Update(int id, [FromBody] object productRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.UpdateProductAsync(id, productRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpPatch("{id}/restock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ReStock(int id, [FromBody] object restockRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.ReStockProductAsync(id, restockRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpPatch("{id}/purchased")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GoodPurchased(int id, [FromBody] object purchaseRequestDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.GoodPurchasedAsync(id, purchaseRequestDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.DeleteProductAsync(id, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpGet]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10, [FromQuery] int? categoryId = null)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return Unauthorized("Authorization token is missing.");
            }

            var (statusCode, content) = await _productServiceClient.GetProductsAsync(page, sizePerPage, categoryId, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var (statusCode, content) = await _productServiceClient.GetProductByIdAsync(id);
            return CreateResponse(statusCode, content);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string? productName,
            [FromQuery] int page = 1,
            [FromQuery] int sizePerPage = 10)
        {
            var (statusCode, content) = await _productServiceClient.SearchProductsAsync(productName, page, sizePerPage);
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
