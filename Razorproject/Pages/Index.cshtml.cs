using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
namespace Razorproject.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;

		public IndexModel(ILogger<IndexModel> logger)
		{
			_logger = logger;
		}

        public string UserName { get; set; }

        public void OnGet()
        {
            var userNameCookie = Request.Cookies["username"];
            if (!string.IsNullOrEmpty(userNameCookie))
            {
                UserName = userNameCookie;
            }
            else
            {
                UserName = "Guest";
            }
        }
    }
}
        
    

