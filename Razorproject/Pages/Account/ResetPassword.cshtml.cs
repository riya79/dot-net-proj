using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Razorproject.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ResetPasswordModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ResetPassword? resetPassword { get; set; }

        public void OnGet(string token, string email)
        {
            resetPassword = new ResetPassword
            {
                Token = token,
                Email = email
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7046/api/Account/resetpassword", resetPassword);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Password reset successful. You can now log in with your new password.";
                return RedirectToPage("/Account/Login");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error resetting password: {errorMessage}");
                return Page();
            }
        }

        public class ResetPassword
        {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            public string? Token { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string? ConfirmPassword { get; set; }
        }
    }
}
