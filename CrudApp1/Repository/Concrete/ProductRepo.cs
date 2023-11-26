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

        public Product GetProductById(int id)
        {
            var prod = dbContext.Products.FirstOrDefault(x => x.Id == id);
            return prod;
        }

        public List<Product> GetProducts()
        {
            var result=  dbContext.Products.ToList();
      
            return result;
        }

        public bool UpdateProduct(Product product)
        {
            var dbProd = dbContext.Products.FirstOrDefault(y => y.Id == product.Id);

            if (dbProd != null)
            {
                dbProd.Name = product.Name; dbProd.UnitInStock = product.UnitInStock; 
                dbProd.UnitPrice=product.UnitPrice;
                if(product.ImageUrl!=null)
                    dbProd.ImageUrl=product.ImageUrl;



                dbContext.SaveChanges();

                return true;
            }
            return false;
        }
    }
}
