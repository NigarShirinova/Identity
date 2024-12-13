using Identity.Areas.Admin.Models.Email;
using Identity.Data;
using Identity.Utilities.EmailHandler.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmailController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmailService _mailService;

        public EmailController(AppDbContext dbContext, IEmailService mailService)
        {
            _dbContext = dbContext;
            _mailService = mailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(EmailSendVM model)
        {
            if (!ModelState.IsValid) return View(model);

          
            var subscribedUsers = _dbContext.Users.Where(user => user.IsSubscribed).ToList();

            foreach (var subscriber in subscribedUsers)
            {
                var message = new Utilities.EmailHandler.Models.Message(
                    new List<string> { subscriber.Email },
                    model.Name,
                    $"{model.Description}"
                );
                _mailService.SendMessage(message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
