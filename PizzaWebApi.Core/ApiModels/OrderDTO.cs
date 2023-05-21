using PizzaWebApi.Core.Models;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// OrderDTO contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.OrderDTOValidator"/>
    /// </remarks>
    public class OrderDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public OrderState State { get; set; }

        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public int? UpdatedBy { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }

        #region Details

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DeliveryType DeliveryType { get; set; }

        #endregion

        public decimal Total { get; set; }

    }
}
