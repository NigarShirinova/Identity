using Identity.Data;
using Identity.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    public class BasketController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public BasketController(Microsoft.AspNetCore.Identity.UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(int productId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) { return Unauthorized(); }

            var product = _context.Products.Find(productId);
            if (product == null) { return NotFound("Mehsul sebete elave oluna bilmedi!"); }

            if (product.StockQuantity < 0) { return NotFound("Mehsul stokda bitib!"); }


            var basket = _context.Baskets.FirstOrDefault(b => b.UserId == user.Id);
            if(basket == null)
            {
                basket = new Basket
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.Now
                };
                _context.Baskets.Add(basket);
            }

            var basketProduct = new BasketProduct
            {
                Basket = basket,
                ProductId = product.Id,
                Quantity = 1,
                CreatedAt = DateTime.Now
            };

            _context.BasketProducts.Add(basketProduct);
            _context.SaveChanges();

            return Ok("mehsul ugurla elave olundu!");
        }
    }
}
