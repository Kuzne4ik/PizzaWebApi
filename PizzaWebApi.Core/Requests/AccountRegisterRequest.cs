namespace PizzaWebApi.Core.Requests
{
    /// <summary>
    /// Тело запроса для регистрации аккаунта нового пользователя
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.AccountRegisterRequestValidator"/>
    /// </remarks>
    public class AccountRegisterRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
