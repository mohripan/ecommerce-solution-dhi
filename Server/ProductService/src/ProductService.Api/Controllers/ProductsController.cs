using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Requests;
using ProductService.Application.Services.Interfaces;

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
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var product = await _productAppService.CreateProductAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDto request)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var product = await _productAppService.UpdateProductAsync(id, request, userId);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPatch("{id}/restock")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ReStock(int id, [FromBody] ReStockRequestDto request)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var product = await _productAppService.ReStockProductAsync(id, request, userId);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPatch("{id}/purchased")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GoodPurchased(int id, [FromBody] ReStockRequestDto request)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var product = await _productAppService.GoodPurchasedAsync(id, request, userId);
            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var success = await _productAppService.DeleteProductAsync(id, userId);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10, [FromQuery] int? categoryId = null)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var products = await _productAppService.GetProductsAsync(userId, page, sizePerPage, categoryId);

            return Ok(products);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst("sub")!.Value);
            var product = await _productAppService.GetProductByIdAsync(id, userId);

            if (product == null) return NotFound();

            return Ok(product);
        }
    }
}
