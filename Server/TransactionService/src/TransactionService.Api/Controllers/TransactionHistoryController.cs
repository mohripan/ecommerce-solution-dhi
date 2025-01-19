using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionService.Application.DTOs.Requests;
using TransactionService.Application.Services.Interfaces;

namespace TransactionService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryAppService _transactionHistoryAppService;

        public TransactionHistoryController(ITransactionHistoryAppService transactionHistoryAppService)
        {
            _transactionHistoryAppService = transactionHistoryAppService;
        }

        [HttpPost]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Create([FromBody] TransactionHistoryRequestDto request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized("User ID not found in token.");

                var transaction = await _transactionHistoryAppService.CreateTransactionAsync(request, int.Parse(userId));
                return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTransactionStatusRequestDto request)
        {
            try
            {
                var transaction = await _transactionHistoryAppService.UpdateTransactionStatusAsync(id, request.StatusId);
                if (transaction == null) return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var transaction = await _transactionHistoryAppService.GetTransactionByIdAsync(id);
                if (transaction == null) return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                    return Unauthorized("User ID not found in token.");

                var transactions = await _transactionHistoryAppService.GetTransactionsByUserIdAsync(int.Parse(userId), page, sizePerPage);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
