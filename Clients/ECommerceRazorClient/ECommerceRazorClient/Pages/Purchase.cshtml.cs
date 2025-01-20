using ECommerceRazorClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ECommerceRazorClient.Pages
{
    public class PurchaseModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int ProductId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal Price { get; set; }

        public async Task<IActionResult> OnPostAsync(int Quantity, string Remarks)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            var userId = JwtHelper.GetClaimFromToken(token, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var payload = new
            {
                productId = ProductId,
                userId = int.Parse(userId),
                price = Price,
                quantity = Quantity,
                remarks = Remarks
            };

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:5001/api/transactions", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/BuyerMenu");
            }

            ModelState.AddModelError("", "Purchase failed. Please try again.");
            return Page();
        }
    }
}
