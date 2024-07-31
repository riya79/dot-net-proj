using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace Razorproject.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete("token");
            HttpContext.Response.Cookies.Delete("username");
            HttpContext.Response.Cookies.Delete("userId");
            HttpContext.Response.Cookies.Delete("userDetails");

            return RedirectToPage("/Account/Login");
        }
    }
}
