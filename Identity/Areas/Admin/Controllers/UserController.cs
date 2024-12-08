using Identity.Areas.Admin.Models.User;
using Identity.Constants;
using Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Areas.Admin.Controllers
{
    
        [Area("Admin")]
        //[Authorize(Roles = "Admin")]
        public class UserController : Controller
        {
            private readonly UserManager<User> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }
            [HttpGet]
            public IActionResult Index()
            {
                var users = new List<UserVM>();
                foreach (var user in _userManager.Users.ToList())
                {
                    if (!_userManager.IsInRoleAsync(user, "Admin").Result)
                    {
                        users.Add(new UserVM
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            Country = user.Country,
                            City = user.City,
                            Roles = _userManager.GetRolesAsync(user).Result.ToList()
                        });
                    }
                }
                var model = new UserIndexVM
                {
                    Users = users
                };
                return View(model);
            }
            [HttpGet]
            public IActionResult Create()
            {
                var model = new UserCreateVM
                {
                    Roles = _roleManager.Roles.Where(r => r.Name != "Admin").Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id
                    }).ToList()
                };
                return View(model);
            }
            [HttpPost]
            public IActionResult Create(UserCreateVM userModel)
            {
                if (!ModelState.IsValid) return View(userModel);

                var user = new User
                {
                    Email = userModel.EmailAddress,
                    Country = userModel.Country,
                    City = userModel.City,
                    UserName = userModel.EmailAddress,
                 
                };

                var result = _userManager.CreateAsync(user, userModel.Password).Result;
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(userModel);
                    }
                }
                foreach (var roleId in userModel.RoleIds)
                {
                    var role = _roleManager.FindByIdAsync(roleId).Result;
                    if (role == null)
                    {
                        ModelState.AddModelError("RoleIds", "Role doesn't exist");
                        return View(userModel);
                    }
                    var addRoleToUserResult = _userManager.AddToRoleAsync(user, role.Name).Result;
                    if (!addRoleToUserResult.Succeeded)
                    {
                        ModelState.AddModelError("RoleIds", "Error occured");
                        return View(userModel);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            [HttpGet]
            public IActionResult Update(string id)
            {
                var user = _userManager.FindByIdAsync(id).Result;
                if (user == null)
                {
                    return NotFound();
                }
                List<string> roleIds = new List<string>();
                var userRoles = _userManager.GetRolesAsync(user).Result;
                foreach (var userRole in userRoles)
                {
                    var role = _roleManager.FindByNameAsync(userRole).Result;
                    roleIds.Add(role.Id);
                }
                var model = new UserUpdateVM
                {
                    Country = user.Country,
                    City = user.City,
                    EmailAddress = user.Email,
           
                    Roles = _roleManager.Roles.Where(r => r.Name != UserRoles.Admin.ToString()).Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id
                    }).ToList(),
                    RoleIds = roleIds
                };

                return View(model);
            }

            [HttpPost]
            public IActionResult Update(string id, UserUpdateVM model)
            {
                if (!ModelState.IsValid) return View(model);
                var user = _userManager.FindByIdAsync(id).Result;
                if (user == null)  return NotFound();

             user.UserName = model.EmailAddress;
             user.Country = model.Country;
                user.City = model.City;
                if (model.Password != null)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
                }
                List<string> RoleIds = new List<string>();

                var userRoles = _userManager.GetRolesAsync(user).Result;
                foreach (var userRole in userRoles)
                {
                    var role = _roleManager.FindByNameAsync(userRole).Result;
                    RoleIds.Add(role.Id);
                }

                var willBeAddedRolesIds = model.RoleIds.Except(RoleIds).ToList();
                var willBeDeletedROlesIds = RoleIds.Except(model.RoleIds).ToList();

                foreach (var roleId in willBeAddedRolesIds)
                {
                    var role = _roleManager.FindByIdAsync(roleId).Result;
                    if (role == null)
                    {
                        ModelState.AddModelError("RoleIds", "Role not found");
                        return View(model);
                    }
                    var result = _userManager.AddToRoleAsync(user, role.Name).Result;
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            return View(model);
                        }
                    }
                }
                foreach (var roleId in willBeDeletedROlesIds)
                {
                    var role = _roleManager.FindByIdAsync(roleId).Result;
                    if (role == null)
                    {
                        ModelState.AddModelError("RoleIds", "Role Not Found");
                        return View(model);
                    }
                    var result = _userManager.RemoveFromRoleAsync(user, role.Name).Result;
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }
                var updateResult = _userManager.UpdateAsync(user).Result;
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }

            [HttpPost]
            public IActionResult Delete(string id)
            {
                var user = _userManager.FindByIdAsync(id).Result;
                if (user == null) return NotFound();

                var result = _userManager.DeleteAsync(user).Result;
                if (!result.Succeeded) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            public IActionResult Details(string id)
            {
                var user = _userManager.FindByIdAsync(id).Result;
                if (user == null) return NotFound();
                var model = new UserDetailVM
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Country = user.Country,
                    City = user.City,

                };
                return View(model);
            }
        }
    }

