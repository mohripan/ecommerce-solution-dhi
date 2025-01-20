using ECommerceRazorClient.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ECommerceRazorClient.Pages
{

    public class MainMenuModel : PageModel
    {
        public string Role { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                Role = JwtHelper.GetClaimFromToken(token, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            }
            catch
            {
                return RedirectToPage("/Login");
            }

            if (Role == "Seller")
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await httpClient.GetAsync("http://localhost:5001/api/products");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    Products = JsonConvert.DeserializeObject<List<Product>>(data.values.ToString());
                }
                else
                {
                    ModelState.AddModelError("", "Failed to load products. Please try again later.");
                }
                return Page();
            }
            else if (Role == "Buyer")
            {
                return RedirectToPage("/BuyerMenu");
            }

            return RedirectToPage("/Login");
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = Request.Cookies["AuthToken"];
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            await httpClient.DeleteAsync($"http://localhost:5001/api/products/{id}");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRestockAsync(int id, int Quantity)
        {
            var token = Request.Cookies["AuthToken"];
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var payload = new { quantity = Quantity };
            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            await httpClient.PatchAsync($"http://localhost:5001/api/products/{id}/restock", content);
            return RedirectToPage();
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
