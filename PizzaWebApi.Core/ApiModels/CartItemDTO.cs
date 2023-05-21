using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// CartItem contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.CartItemDTOValidator"/>
    /// </remarks>
    public class CartItemDTO
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int ProductId { get; set; }

        public ProductDTO Product { get; set; }
    }
}
