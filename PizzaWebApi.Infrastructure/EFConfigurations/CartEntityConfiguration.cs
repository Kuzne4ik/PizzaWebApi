using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.EFConfigurations
{
    public class CartEntityConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.Created).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(p => p.CreatedBy).IsRequired();
        }
    }
}
