namespace PizzaWebApi.Core.Interfaces
{
    public interface IPromoCodeService
    {
        decimal CarculateDiscount(string promoCode, decimal sum);
    }
}
