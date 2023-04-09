using PizzaWebApi.SharedKernel;

namespace PizzaWebApi.Core.Models
{
    public  class Cart : AuditEntity
    {
        public int UserId { get; set; }
        public string? PromoCode { get; set; }

        //У других ICollection ?
        public List<CartItem>  CartItems { get; set; }
    }
}
