namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// Cart Data contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.CartDTOValidator"/>
    /// </remarks>
    public class CartDTO
    {
        public int Id { get; set; }

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
