using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Infrastructure.EFConfigurations;

namespace PizzaWebApi.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CartEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        }
    }
}
