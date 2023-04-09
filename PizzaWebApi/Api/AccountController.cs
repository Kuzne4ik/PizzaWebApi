using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Web.Api
{
    public class AccountController : BaseApiController
    {
        IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// User Register
        /// </summary>
        /// <param name="accountRegisterRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("/register", Name = "Register")]
        public Task<bool> RegisterAsync(AccountRegisterRequest accountRegisterRequest)
        {
            return _accountService.RegisterAsync(accountRegisterRequest);
        }

        [AllowAnonymous]
        [HttpPost("/login", Name = "Login")]
        public Task<TokenDTO> LoginAsync(LoginRequest loginRequest)
        {
            return _accountService.LoginAsync(loginRequest);
        }

        [AllowAnonymous]
        [HttpPost("/token-refresh", Name = "TokenRefresh")]
        public Task<TokenDTO> TokenRefreshAsync(TokenRefreshRequest tokenRefreshRequest)
        {
            return _accountService.TokenRefreshAsync(tokenRefreshRequest);
        }

        /// <summary>
        /// Revoke User (RefreshToken set null)
        /// </summary>
        /// <param name="userName"></param>
        [Authorize]
        [HttpPost("revoke/{username}", Name = "Revoke")]
        public void RevokeAsync(string userName)
        {
            _accountService.RevokeAsync(userName);
        }
    }
}
