using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// Cart Data contract
    /// </summary>
    public class CartDTO
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public string PromoCode { get; set; }
        public decimal Total { get; set; }
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public int? UpdatedBy { get; set; }

        public ICollection<CartItemDTO> CartItems { get; set; }
    }
}
