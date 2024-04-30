using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name ="Confirm Password")]
        [Compare("Password",ErrorMessage ="Password and Confirm Password must be same")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
    }
}
