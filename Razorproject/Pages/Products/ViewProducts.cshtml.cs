using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Razorproject.Pages.Products
{
    public class ViewProductsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
       
        public ViewProductsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
          
        }

        public List<Product> Products { get; set; } = new List<Product>();

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Request.Cookies["token"];
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"https://localhost:7046/api/Products/user-products");

            if (response.IsSuccessStatusCode)
            {
                Products = await response.Content.ReadFromJsonAsync<List<Product>>();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the products.");
            }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal CustomerPrice { get; set; }
        }
    }
}

