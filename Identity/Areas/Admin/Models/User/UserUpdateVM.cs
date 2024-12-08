using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.User
{
    public class UserUpdateVM
    {
        public UserUpdateVM()
        {
            RoleIds = new List<string>();
        }
        [Required]
        [EmailAddress]

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm password must be same")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Country must be entered")]
        public string Country { get; set; }
        [Required(ErrorMessage = "City must be entered")]
        public string City { get; set; }

        public List<SelectListItem>? Roles { get; set; }
        public List<string>? RoleIds { get; set; }
    }
}
