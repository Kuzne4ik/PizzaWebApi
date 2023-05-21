namespace PizzaWebApi.Core.Requests
{
    /// <summary>
    /// Тело запроса для обновления Токена
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.TokenRefreshRequestValidator"/>
    /// </remarks>
    public class TokenRefreshRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
