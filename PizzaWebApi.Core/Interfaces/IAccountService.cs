using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Core.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(AccountRegisterRequest accountRegisterRequest);
        Task<TokenDTO> LoginAsync(LoginRequest loginRequest);
        Task<TokenDTO> TokenRefreshAsync(TokenRefreshRequest tokenRefreshRequest);
        Task RevokeAsync(string userName);

        Task<bool> RegisterAdmin(AccountRegisterRequest accountRegisterRequest);
    }
}
