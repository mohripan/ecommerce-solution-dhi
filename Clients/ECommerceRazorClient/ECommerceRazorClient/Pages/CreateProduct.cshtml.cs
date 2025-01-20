using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ECommerceRazorClient.Pages
{
    public class CreateProductModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync(string Name, int CategoryId, decimal Price)
        {
            var token = Request.Cookies["AuthToken"];
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var payload = new { name = Name, categoryId = CategoryId, price = Price };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:5001/api/products", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/MainMenu");
            }

            ModelState.AddModelError("", "Error creating product.");
            return Page();
        }
    }
}
