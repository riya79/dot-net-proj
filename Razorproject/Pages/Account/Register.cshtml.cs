using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApiRoleBasedAuthorization.Model;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Razorproject.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Register Model { get; set; }

        public RegisterModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public void OnGet()
        {
            Model = new Register();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var apiEndpoint = "https://localhost:7046/api/Account/register";
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(apiEndpoint, Model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Registration successful.";
                return RedirectToPage("/Account/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Please try again later.");
                return Page();
            }
        }
    }
}
