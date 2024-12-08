using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AccountLoginVM = Identity.Areas.Admin.Models.Account.AccountLoginVM;

namespace Identity.Areas.Admin.Models
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email ve ya sifre yalnisdir");
                return View(model);
            }

            if(!_userManager.IsInRoleAsync(user, "Admin").Result)
            {
                ModelState.AddModelError(string.Empty, "Email ve ya sifre yalnisdir");
                return View(model);
            }

            var result = _signInManager.CheckPasswordSignInAsync(user, model.Password, false).Result;

            if (!result.Succeeded)
            {

                ModelState.AddModelError(string.Empty, "Ugursuz login cehdi!");
                return View(model);
            }


            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


    }
}
