using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace Razorproject.Pages.Account
{
    
        public class LoginModel : PageModel
        {
            private readonly IHttpClientFactory _httpClientFactory;

            [BindProperty]
            public LoginInputModel? Input { get; set; }
            public string ErrorMessage { get; set; }


        public LoginModel(IHttpClientFactory httpClientFactory)
            {
                _httpClientFactory = httpClientFactory;
            }

            public void OnGet()
            {
            }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid)
                {
                ErrorMessage = "Invalid login attempt.";
                return RedirectToPage("/Error");
                }

                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsJsonAsync("https://localhost:7046/api/Account/login", Input);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    
                // Store token and username in local storage
                // Store token and username in cookies
                HttpContext.Response.Cookies.Append("token", result.Token);
                HttpContext.Response.Cookies.Append("username", result.Username);
                HttpContext.Response.Cookies.Append("userId" , result.UserId);
                TempData["Message"] = "Login successful.";

                /* var userDetailsResponse = await client.GetAsync($"https://localhost:7046/api/User/userdetails/{result.Username}");

                     if (userDetailsResponse.IsSuccessStatusCode)
                     {
                         var userDetails = await userDetailsResponse.Content.ReadFromJsonAsync<UserDetails>();
                     var userDetailsJson = JsonSerializer.Serialize(userDetails);

                     // Store user details JSON in a cookie
                     HttpContext.Response.Cookies.Append("userDetails", userDetailsJson);

                     return RedirectToPage("/Account/UserInfo");

                     }
                     else
                     {
                         ModelState.AddModelError(string.Empty, "Error fetching user details.");
                     }*/
                // Check roles
                if (result.Roles.Contains("Admin"))
                {

                    return RedirectToPage("/User/ViewAllUsers"); 
                }
                else
                {
                    ErrorMessage = "Invalid login credentials.";
                    return RedirectToPage("/Index"); 
                }
            }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

                return Page();
            }

            public class LoginInputModel
            {
                [Required]
                public string? Username { get; set; }
            [Required]
            public string? Password { get; set; }
            }

            public class LoginResult
            {
                public string? UserId {  get; set; }
                public string? Token { get; set; }
                public string? Username { get; set; }
                public List<string>? Roles { get; set; }
        }

        }
    }

