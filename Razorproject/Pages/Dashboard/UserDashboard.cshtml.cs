using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razorproject.Pages.Dashboard
{
    public class UserDashboardModel : PageModel
    {
        public string Username { get; set; }

        public void OnGet()
        {
            // Replace with actual user details retrieval logic
            Username = "TestUser";
        }
    
    }
}
