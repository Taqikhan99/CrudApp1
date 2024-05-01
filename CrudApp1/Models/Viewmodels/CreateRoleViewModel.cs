using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace CrudApp1.Models.Viewmodels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
