using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.EFConfigurations
{
    public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasOne(p => p.Cart).WithMany(p => p.CartItems).HasForeignKey(s => s.CartId).IsRequired();
            builder.HasOne(p => p.Product).WithMany(p => p.CartItems).HasForeignKey(s => s.ProductId).IsRequired();
            builder.Property(p => p.Created).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
