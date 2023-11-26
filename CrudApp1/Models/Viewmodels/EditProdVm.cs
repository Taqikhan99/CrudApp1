using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class EditProdVm
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int UnitPrice { get; set; }
        [Required]
        public int UnitInStock { get; set; }

        //[BindNever]//ignore this
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
    }
}
