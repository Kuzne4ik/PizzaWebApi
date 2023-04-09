using PizzaWebApi.SharedKernel;

namespace PizzaWebApi.Core.Models
{
    public class PromoCode : BaseEntity
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Code { get; set; }

        public decimal Percent { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

    }
}
