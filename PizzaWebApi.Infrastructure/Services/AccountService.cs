using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PizzaWebApi.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        ILogger<AccountService> _logger;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="accountRegisterRequest"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<bool> RegisterAsync(AccountRegisterRequest accountRegisterRequest)
        {
            _logger.LogInformation($"{nameof(RegisterAsync)} run");
            var user = new ApplicationUser()
            {
                UserName = accountRegisterRequest.UserName,
                Name = accountRegisterRequest.Name,
                Email = accountRegisterRequest.Email
            };
            var result = await _userManager.CreateAsync(user, accountRegisterRequest.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Registeration fail, Error: {result.Errors.First().Description}");
            }

            //Init default Roles
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>TokenDTO</returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task<TokenDTO> LoginAsync(LoginRequest loginRequest)
        {
            _logger.LogInformation($"{nameof(LoginAsync)} run");

            ApplicationUser user;
            if (loginRequest.UserNameOrEmail.Contains("@"))
                user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail);
            else
                user = await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                throw new AuthenticationException("Login or password is invalid");
            }
            try
            {
                var claims = await GetClaims(user);

                var token = GenerateToken(claims);

                user.RefreshToken = GenerateRefreshToken();
                user.Expire = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expires"]));

                await _userManager.UpdateAsync(user);

                return new TokenDTO()
                {
                    Token = token,
                    RefreshToken = user.RefreshToken,
                    Expires = user.Expire
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(LoginAsync)} exception");
                throw new ApplicationException("Login failed");
            }
        }

        /// <summary>
        /// Generate new token by RefreshToken
        /// </summary>
        /// <param name="tokenRefreshRequest"></param>
        /// <returns>TokenDTO</returns>
        /// <exception cref="SecurityTokenException"></exception>
        public async Task<TokenDTO> TokenRefreshAsync(TokenRefreshRequest tokenRefreshRequest)
        {
            _logger.LogInformation($"{nameof(TokenRefreshAsync)} run");

            var principal = GetPrincipalFromExpiredToken(tokenRefreshRequest.Token);

            string userName = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new SecurityTokenException("Invalid token");
            }
            if (user.RefreshToken != tokenRefreshRequest.RefreshToken)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            try
            {
                Dictionary<string, object> claims = await GetClaims(user);

                var newToken = GenerateToken(claims);
                var newRefreshToken = GenerateRefreshToken();
                var newExpire = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expires"]));

                user.RefreshToken = newRefreshToken;
                user.Expire = newExpire;

                await _userManager.UpdateAsync(user);

                return new TokenDTO()
                {
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Expires = newExpire
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(TokenRefreshAsync)} exception");
                throw new ApplicationException("Token refresh failed");
            }
        }

        /// <summary>
        /// Cancel Refresh token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public async Task RevokeAsync(string username)
        {
            _logger.LogInformation($"{nameof(RevokeAsync)} run");

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentException("Username is invalid");
            }
            try
            {
                user.RefreshToken = null;
                _ = await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(RevokeAsync)} exception");
                throw new ApplicationException("Token revoke failed");
            }
        }

        /// <summary>
        /// Register Admin
        /// </summary>
        /// <param name="accountRegisterRequest"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="BadHttpRequestException"></exception>
        public async Task<bool> RegisterAdmin(AccountRegisterRequest accountRegisterRequest)
        {
            _logger.LogInformation($"{nameof(RegisterAdmin)} run");

            var userExists = await _userManager.FindByNameAsync(accountRegisterRequest.UserName);
            if (userExists != null)
                throw new InvalidOperationException($"User {accountRegisterRequest.UserName} exists!");

            var user = new ApplicationUser()
            {
                UserName = accountRegisterRequest.UserName,
                Name = accountRegisterRequest.Name,
                Email = accountRegisterRequest.Email
            };
            try
            {
                var result = await _userManager.CreateAsync(user, accountRegisterRequest.Password);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Registeration fail, Error: {result.Errors.First().Description}");
                }

                //Init default Roles
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(RegisterAdmin)} exception");
                throw new ApplicationException("Register admin failed");
            }
            return true;
        }

        /// <summary>
        /// Grt JWT token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        internal string GenerateToken(Dictionary<string, object> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expires"])),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(descriptor);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        /// <summary>
        /// Get data from JWT token and validate
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        /// <summary>
        /// Get Refresh Token
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Get Claims dict
        /// </summary>
        /// <param name="user">ApplicationUser</param>
        /// <returns></returns>
        private async Task<Dictionary<string, object>> GetClaims(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new Dictionary<string, object>
                {
                    { ClaimTypes.Sid, user.Id},
                    { ClaimTypes.Name, user.UserName},
                    { ClaimTypes.GivenName, $"{user.Name}"}
                };
            foreach (var userRole in roles)
            {
                claims.Add(ClaimTypes.Role, userRole);
            }

            return claims;
        }
    }
}
