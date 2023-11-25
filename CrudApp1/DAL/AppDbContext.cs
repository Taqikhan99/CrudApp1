using CrudApp1.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudApp1.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

    }
}
