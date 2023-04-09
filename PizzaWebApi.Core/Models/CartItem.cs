using PizzaWebApi.SharedKernel;

namespace PizzaWebApi.Core.Models
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }

        public int Quantity { get; set; }

        public DateTime Created { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
        public Cart Cart { get; set; }
    }
}
