using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Role Name is Required")]
        
        public string Name { get; set; }    

        public List<string> UsersInRole { get; set; }= new List<string>();
    }
}
