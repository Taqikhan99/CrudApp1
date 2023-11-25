using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class CreateProdVm
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int UnitPrice { get; set; }
        [Required]
        public int UnitInStock { get; set; }
        public string ImageUrl { get; set; }
    }
}
