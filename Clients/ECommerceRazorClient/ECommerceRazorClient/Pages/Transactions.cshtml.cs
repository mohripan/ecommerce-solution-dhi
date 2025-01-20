using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ECommerceRazorClient.Pages
{
    public class TransactionsModel : PageModel
    {
        public List<ProductTransaction> Transactions { get; set; } = new List<ProductTransaction>();

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.GetAsync("http://localhost:5001/api/transactions/seller/product-transactions");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                Transactions = JsonConvert.DeserializeObject<List<ProductTransaction>>(data.values.ToString());
            }

            return Page();
        }
    }

    public class ProductTransaction
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<TransactionHistory> TransactionHistory { get; set; } = new List<TransactionHistory>();
    }

    public class TransactionHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string StatusName { get; set; }
        public string Remarks { get; set; }
        public string TransactionAt { get; set; }
        public string ModifiedOn { get; set; }
    }
}
