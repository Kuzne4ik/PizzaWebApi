namespace PizzaWebApi.Core.Requests
{
    public class LoginRequest
    {
        /// <summary>
        /// UserName or Email
        /// </summary>
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
