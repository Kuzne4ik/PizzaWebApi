namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// OrderItemDTO contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.OrderItemDTOValidator"/>
    /// </remarks>
    public class OrderItemDTO
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public ProductDTO Product { get; set; }
    }
}
