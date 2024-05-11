using CrudApp1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrudApp1.DAL
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //by default ef core has cascading behavior e.g if rec delete from parent table, its dependent recs in child table are also deleted.
            //we change here to Restrict(No Action). Now must be deleted from child table first.
            foreach(var fk in builder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys())){
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }
    }
}
