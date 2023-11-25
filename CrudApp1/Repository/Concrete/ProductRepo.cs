using CrudApp1.DAL;
using CrudApp1.Models;
using CrudApp1.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace CrudApp1.Repository.Concrete
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext dbContext;

        public ProductRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //adding a product
        public bool AddProduct(Product product)
        {
            dbContext.Products.Add(product);
            if(dbContext.SaveChanges()>0)  
                return true;
            return false;
        }

        public List<Product> GetProducts()
        {
            var result=  dbContext.Products.ToList();
      
            return result;
        }
    }
}
