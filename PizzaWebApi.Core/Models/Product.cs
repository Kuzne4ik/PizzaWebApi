using Microsoft.EntityFrameworkCore;
using PizzaWebApi.SharedKernel;

namespace PizzaWebApi.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(Title), IsUnique = true)]
    public class Product : BaseEntity
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public Category Category { get; set; }

        public List<CartItem> CartItems { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}