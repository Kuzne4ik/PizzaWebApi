using PizzaWebApi.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    public class OrderDTO
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public OrderState State { get; set; }

        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? Updated { get; set; }
        public int? UpdatedBy { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }

        #region Details

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string Surname { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public string Address { get; set; }

        public DeliveryType DeliveryType { get; set; }

        #endregion

        [Required]
        public decimal Total { get; set; }

    }
}
