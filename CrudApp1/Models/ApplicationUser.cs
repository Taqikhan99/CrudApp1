using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models
{
    public class ApplicationUser: IdentityUser
    {
        [MaxLength(100)]
        public string City { get; set; }
        
    }
}
