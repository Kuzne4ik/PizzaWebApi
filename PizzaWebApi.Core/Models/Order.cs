using PizzaWebApi.SharedKernel;

namespace PizzaWebApi.Core.Models
{
    public  class Order : AuditEntity
    {
        public int UserId { get; set; }

        public OrderState State { get; set; }

        #region Details

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DeliveryType DeliveryType { get; set; }

        //public PaymentType PaymentType { get; set; }

        #endregion

        /// <summary>
        /// Order Total sum
        /// </summary>
        public decimal Total { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
