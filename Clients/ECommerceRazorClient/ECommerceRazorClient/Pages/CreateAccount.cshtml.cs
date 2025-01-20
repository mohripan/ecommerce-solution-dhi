using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;

namespace ECommerceRazorClient.Pages
{
    public class CreateAccountModel : PageModel
    {
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }
        [BindProperty] public int RoleId { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            var httpClient = new HttpClient();
            var payload = new { username = Username, email = Email, password = Password, roleId = RoleId };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:5001/api/users", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Login");
            }

            ModelState.AddModelError(string.Empty, "Error creating account.");
            return Page();
        }
    }
}
