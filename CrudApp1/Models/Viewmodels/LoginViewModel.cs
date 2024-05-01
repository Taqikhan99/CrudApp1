using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remember Me")]
        public bool RememberMe { get; set;}
    }
}
