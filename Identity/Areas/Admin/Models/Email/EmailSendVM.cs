using System.ComponentModel.DataAnnotations;

namespace Identity.Areas.Admin.Models.Email
{
    public class EmailSendVM
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
