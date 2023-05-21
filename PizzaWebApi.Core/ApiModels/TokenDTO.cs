namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// TokenDTO contract (generate at Server side only)
    /// </summary>
    public class TokenDTO
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }
    }
}
