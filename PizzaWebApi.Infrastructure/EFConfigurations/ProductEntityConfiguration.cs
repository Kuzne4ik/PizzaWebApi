using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.EFConfigurations
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasColumnType("nvarchar").HasMaxLength(250).IsRequired();
            builder.Property(p => p.Title).HasColumnType("nvarchar").HasMaxLength(50).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18, 4)").IsRequired();
            builder.HasOne(g => g.Category).WithMany(g => g.Products).HasForeignKey(s => s.CategoryId);
        }
    }
}
