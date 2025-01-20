using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ECommerceRazorClient.Pages
{
    public class BuyerMenuModel : PageModel
    {
        public List<Search> Products { get; set; } = new List<Search>();
        public bool SearchPerformed { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"http://localhost:5001/api/products/search?productName={Search}&page=1&sizePerPage=10");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    Products = JsonConvert.DeserializeObject<List<Search>>(data.values.ToString());
                    SearchPerformed = true;
                }
            }

            return Page();
        }
    }

    public class Search
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
