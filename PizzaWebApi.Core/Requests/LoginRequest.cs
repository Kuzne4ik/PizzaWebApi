namespace PizzaWebApi.Core.Requests
{
    /// <summary>
    /// Тело запроса для аудентификации пользователя
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.LoginRequestValidator"/>
    /// </remarks>
    public class LoginRequest
    {
        /// <summary>
        /// UserName or Email
        /// </summary>
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
