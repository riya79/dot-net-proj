using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiRoleBasedAuthorization.Data;
using WebApiRoleBasedAuthorization.Model;
using WebApiRoleBasedAuthorization.Model.DTO;


namespace WebApiRoleBasedAuthorization.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _prodcontext;

        public ProductsController(AppDbContext context)
        {
            _prodcontext = context;
        }

       
      
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            List<Product> products = await _prodcontext.Products.ToListAsync();


            return Ok(products);
        }

        [Authorize(Roles = "User")]
        [HttpGet("user-products")]
        
        public async Task<IActionResult> GetProductByUser()
        {
            List<Product> products = await _prodcontext.Products.ToListAsync();

            foreach (var product in products)
            {
                
                product.CustomerPrice = product.CustomerPrice * 0.9m; 
            }
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<Product>> CreateProduct(ProductDto product)
        {

            
          var existingProduct = await _prodcontext.Products.FirstOrDefaultAsync(s => s.Name == product.Name);

            if (existingProduct != null)
            {
                return BadRequest("Product already exists");
            }
            Product newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                CustomerPrice = product.CustomerPrice,

            };
            _prodcontext.Products.Add(newProduct);
            await _prodcontext.SaveChangesAsync();
            return Ok(newProduct);
        }

       
        [HttpGet("{id}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _prodcontext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

       
      

       
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _prodcontext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _prodcontext.Products.Remove(product);
            await _prodcontext.SaveChangesAsync();

            return NoContent();
        }

       
    }
}
