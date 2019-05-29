using System.Data.Entity;

namespace obiektowe
{
    public class ContextDB : DbContext
    {
        public ContextDB() : base("name=DefaultConnection")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
