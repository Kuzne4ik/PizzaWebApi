using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Infrastructure.EFConfigurations
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.Created).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(p => p.CreatedBy).IsRequired();

            builder.Property(p => p.FirstName).HasColumnType("nvarchar").HasMaxLength(250).IsRequired();
            builder.Property(p => p.LastName).HasColumnType("nvarchar").HasMaxLength(250).IsRequired();
            builder.Property(p => p.SurName).HasColumnType("nvarchar").HasMaxLength(250);
            builder.Property(p => p.Phone).IsRequired();
            builder.Property(p => p.Email).IsRequired();
            builder.Property(p => p.Total).HasColumnType("decimal(18, 4)").IsRequired();
        }
    }
}
