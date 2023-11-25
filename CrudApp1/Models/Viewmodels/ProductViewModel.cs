using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models.Viewmodels
{
    public class ProductViewModel
    {

        
        public string Name { get; set; }
        
        public int UnitPrice { get; set; }
       
        public int UnitInStock { get; set; }
        public string ImageUrl { get; set; }
    }
}
