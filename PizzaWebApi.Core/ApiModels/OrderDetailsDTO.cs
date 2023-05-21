using PizzaWebApi.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// OrderDetailsDTO contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.OrderDetailsDTOValidator"/>
    /// </remarks>
    public class OrderDetailsDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string SurName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public string Address { get; set; }

        public PaymentType PaymentType { get; set; }

        public DeliveryType DeliveryType { get; set; }
    }
}
