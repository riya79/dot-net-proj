using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Razorproject.Pages.Account
{
    public class UserInfoModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserInfoModel> _logger;

        public UserInfoModel(IHttpClientFactory httpClientFactory, ILogger<UserInfoModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public UserDetails? userDetails { get; set; } // Correctly named property
       
        public async Task<IActionResult> OnGetAsync(string userid)
        {
           try{ 
            var token = HttpContext.Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Account/Login");
            }


            /*var username = HttpContext.Request.Cookies["username"];*/
            if (string.IsNullOrEmpty(userid))
            {
                ModelState.AddModelError(string.Empty, "Userid not found.");
                return Page();
            }



            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"https://localhost:7046/api/User/userinfo/{userid}");
            if (response.IsSuccessStatusCode)
            {
                userDetails = await response.Content.ReadFromJsonAsync<UserDetails>();
                    Console.WriteLine(userDetails);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error fetching user details.");
            } }
            catch (HttpRequestException ex)
            {
                // Handle network errors
                ModelState.AddModelError(string.Empty, "Network error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle general errors
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return Page();
        }

      

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("OnPostAsync called");

            try
            {
                var token = HttpContext.Request.Cookies["token"];
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Token is missing. Redirecting to login.");
                    return RedirectToPage("/Account/Login");
                }

                // Update the user's details
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync("https://localhost:7046/api/User/updateUserById", userDetails);

                if (response.IsSuccessStatusCode)
                {

                    TempData["Message"] = "User details updated successfully.";
                    TempData["RedirectDelay"] = 120000; // Delay in milliseconds (e.g., 3000ms = 3 seconds)
                    return RedirectToPage("/User/ViewAllUsers");


                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error updating user details.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred in OnPostAsync");
                ModelState.AddModelError(string.Empty, "Network error occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in OnPostAsync");
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }
            return RedirectToPage();
        }

        public class UserDetails
        {
            [Required(ErrorMessage = "User ID is required.")]
            public string? Id { get; set; }
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username can't be longer than 50 characters.")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string? Email { get; set; }
        }
    }
}
