using Identity;
using Identity.Data;
using Identity.Entities;
using Identity.Utilities.EmailHandler.Concrete;
using Identity.Utilities.EmailHandler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

var emailConfiguration = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfiguration);
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

app.MapControllerRoute
    (
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
    );
//TestEmailService(emailConfiguration);

app.MapControllerRoute(name: "default", pattern: "{controller=Account}/{action=register}");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

using(var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

    DbInitializer.Seed(roleManager, userManager);
}


app.Run();


//void TestEmailService(EmailConfiguration emailConfig)
//{
//    try
//    {
//        var emailService = new EmailService(emailConfig);

//        var testMessage = new Message(
//           new List<string> { "aliibrah30@gmail.com" },
//           "Test Email",
//           "Salam PB401!"
//       );


//        emailService.SendMessage(testMessage);
//        Console.WriteLine("Test email sent successfully!");
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error sending test email: {ex.Message}");
//    }
//}

//TestEmailService(emailConfiguration);
