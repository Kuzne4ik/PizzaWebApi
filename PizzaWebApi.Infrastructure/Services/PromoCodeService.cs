using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Services
{
    public class PromoCodeService : IPromoCodeService
    {
        private List<PromoCode> _promoCodes = new List<PromoCode>
        {
            new PromoCode
            {
                Name = "VIP",
                Title = "VIP",
                Code = "VIP",
                Percent = 0.12m,
                Start = DateTime.Now.AddDays(-1),
                End = DateTime.MaxValue
            },
            new PromoCode
            {
                Name = "Any",
                Title = "Any",
                Code = "ANY",
                Percent = 0.1m,
                Start = DateTime.Now.AddDays(-1),
                End = DateTime.MaxValue
            }
        };

        public decimal CarculateDiscount(string promoCode, decimal sum)
        {
            promoCode = promoCode.Trim();
            if (string.IsNullOrEmpty(promoCode))
                return decimal.Zero;

            var percent = GetPromoCodeDiscount(promoCode);
            return sum * percent;
        }

        private decimal GetPromoCodeDiscount(string promoCode)
        {
            var promo = _promoCodes.FirstOrDefault(t => t.Code.ToLower() == promoCode.ToLower() && t.Start < DateTime.UtcNow && t.End > DateTime.UtcNow);
            if (promo != null)
                return promo.Percent;
            return decimal.Zero;
        }
    }
}
