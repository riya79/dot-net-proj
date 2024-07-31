using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace Razorproject.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ForgotPasswordModel (IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ForgotPassword? ForgotPass { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7046/api/Account/forgotpassword", ForgotPass);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "If an account with that email exists, a password reset link has been sent.";
                return RedirectToPage("/Account/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error sending password reset email.");
                return Page();
            }
        }

        public class ForgotPassword
        {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }
            public string? ClientUri { get; set; } = "https://localhost:7046/api/Account/resetpassword";
        }
    }
}
