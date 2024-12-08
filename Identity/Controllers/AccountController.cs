using Identity.Entities;
using MimeKit;

using Identity.Utilities.EmailHandler.Models;
using Identity.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using AccountLoginVM = Identity.ViewModels.Account.AccountLoginVM;
using NETCore.MailKit.Core;
using IEmailService = Identity.Utilities.EmailHandler.Abstract.IEmailService;

namespace Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email, 
                Email = model.Email,
                City = model.City,
                Country = model.Country,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

          
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

          
            var url = Url.Action(
                nameof(ConfirmEmail),
                "Account",
                new { token, email = user.Email },
                Request.Scheme
            );
           
            _emailService.SendMessage(new Message(
                new List<string> { user.Email },
                "Hesab Tesdiqi",
                $"Zehmet olmasa linke tiklayin ve hesabinizi tesdiq edin: {url}"
            ));

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return NotFound("Tesdiqlemek mumkun olmadi");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return View("Error", result.Errors);
            }

            return RedirectToAction(nameof(Login));
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

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Email tesdiqlenmeyib");
                return View(model);
            }

            var result = _signInManager.CheckPasswordSignInAsync(user, model.Password,  false).Result;

            if (!result.Succeeded)
            {

                ModelState.AddModelError(string.Empty, "Ugursuz login cehdi!");
                return View(model);
            }
          
         
            return RedirectToAction("Index", "Home");
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
