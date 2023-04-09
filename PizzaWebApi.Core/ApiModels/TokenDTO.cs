namespace PizzaWebApi.Core.ApiModels
{
    public class TokenDTO
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }
    }
}
