using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Requests;
using ProductService.Application.Services.Interfaces;
using System.Security.Claims;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductAppService _productAppService;

        public ProductsController(IProductAppService productAppService)
        {
            _productAppService = productAppService;
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([FromBody] ProductRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var product = await _productAppService.CreateProductAsync(request, int.Parse(userId));
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var product = await _productAppService.UpdateProductAsync(id, request, int.Parse(userId));
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPatch("{id}/restock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ReStock(int id, [FromBody] ReStockRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var product = await _productAppService.ReStockProductAsync(id, request, int.Parse(userId));
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPatch("{id}/purchased")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GoodPurchased(int id, [FromBody] ReStockRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var product = await _productAppService.GoodPurchasedAsync(id, request, int.Parse(userId));
                if (product == null) return NotFound();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var success = await _productAppService.DeleteProductAsync(id, int.Parse(userId));
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10, [FromQuery] int? categoryId = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var products = await _productAppService.GetProductsAsync(int.Parse(userId), page, sizePerPage, categoryId);

            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productAppService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string? productName,
            [FromQuery] int page = 1,
            [FromQuery] int sizePerPage = 10)
        {
            var products = await _productAppService.SearchProductsAsync(productName, page, sizePerPage);
            return Ok(products);
        }
    }
}
