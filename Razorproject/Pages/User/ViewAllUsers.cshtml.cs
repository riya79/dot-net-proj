using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;

namespace Razorproject.Pages.User
{
    public class ViewAllUsersModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ViewAllUsersModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<UserDto> Users { get; set; } = new List<UserDto>();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync("https://localhost:7046/api/User/allusers");

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
                Users = users ?? new List<UserDto>();

                return Page();
            }

            return RedirectToPage("/Error");
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                var response = await client.DeleteAsync($"https://localhost:7046/api/User/DeleteUser/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "User deleted successfully.";

                    await OnGetAsync();
                    return Page();
                }
                else
                {
                    TempData["ErrorMessage"] = "Error deleting user.";
                }
                return RedirectToPage("/Error");
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = "Network error occurred: " + ex.Message;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred: " + ex.Message;
                return RedirectToPage();
            }
        }


        public class UserDto
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        }
    }
}


