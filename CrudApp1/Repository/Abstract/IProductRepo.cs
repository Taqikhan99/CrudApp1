using CrudApp1.Models;

namespace CrudApp1.Repository.Abstract
{
    public interface IProductRepo
    {
        List<Product> GetProducts();

        bool AddProduct(Product product);
    }
}
