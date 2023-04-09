using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.EFConfigurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(p => p.Quantity).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18, 4)").IsRequired();
            builder.HasOne(p => p.Order).WithMany(p => p.OrderItems).HasForeignKey(s => s.OrderId).IsRequired();
            builder.HasOne(p => p.Product).WithMany(p => p.OrderItems).HasForeignKey(s => s.ProductId).IsRequired();
        }
    }
}
