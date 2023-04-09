using Moq;
using PizzaWebApi.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Infrastructure.Services;
using System;
using PizzaWebApi.Core.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace PizzaWebApi.Tests.Services
{
    public class AccountServiceTest
    {
        private Mock<UserManager<ApplicationUser>> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        private Mock<ILogger<AccountService>> _logger;
        private IConfiguration _configuration;

        //Arrange
        private Dictionary<string, string> _inMemorySettings = new Dictionary<string, string> {
              {"Jwt:Issuer", "WebApiJwt.com"},
              {"Jwt:Audience", "localhost"},
              {"Jwt:Key", "secret12345678910"},
              {"Jwt:Expires", "1"}
        };

        private List<ApplicationUser> _users = new List<ApplicationUser>
        {
            new ApplicationUser { 
                Id = "1",
                Email = "test@test.com"
            }
        };

        private List<IdentityRole> _roles = new List<IdentityRole>
        {
            new IdentityRole {
                Id = "1",
                Name = UserRoles.Admin
            },
            new IdentityRole {
                Id = "1",
                Name = UserRoles.User
            }
        };

        public AccountServiceTest()
        {
            _userManager = MockUserManager<ApplicationUser>(_users);
            _roleManager = MockRoleManager<IdentityRole>(_roles).Object;
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_inMemorySettings)
                .Build();
            _logger = new Mock<ILogger<AccountService>>();
        }


        [Fact]
        public async Task RegisterTest()
        {
            // # Arrange
            var accoutService = new AccountService(_userManager.Object, _roleManager, _configuration, _logger.Object);

            // # Act
            var result = await accoutService.RegisterAsync(new AccountRegisterRequest
            {
                Email = "test@test.com",
                Name = "Test",
                Password = "1",
                UserName = "Test"
            });

            // # Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoginTest()
        {
            // # Arrange
            var accoutService = new AccountService(_userManager.Object, _roleManager, _configuration, _logger.Object);
            var loginRequest = new LoginRequest
            {
                UserNameOrEmail = "test@test.com",
                Password = "1"
            };
             

            // # Act
            var tokenObject = await accoutService.LoginAsync(loginRequest);

            // # Assert
            Assert.NotNull(tokenObject);
            Assert.NotNull(tokenObject.Token);
            Assert.NotNull(tokenObject.RefreshToken);
            Assert.True(DateTime.Now > tokenObject.Expires);
        }

        [Fact]
        public async Task TokenRefreshTest()
        {
            // # Arrange
            var accoutService = new AccountService(_userManager.Object, _roleManager, _configuration, _logger.Object);
            var loginRequest = new LoginRequest
            {
                UserNameOrEmail = "test@test.com",
                Password = "1"
            };

            // # Act
            var tokenObject = await accoutService.LoginAsync(loginRequest);

            // # Assert
            Assert.NotNull(tokenObject);

            // # Arrange
            var tokenRefreshRequest = new TokenRefreshRequest
            {
                Token = tokenObject.Token,
                RefreshToken = tokenObject.RefreshToken
            };

            // # Act
            var newTokenObj = await accoutService.TokenRefreshAsync(tokenRefreshRequest);

            // # Assert
            Assert.NotNull(newTokenObj);
            Assert.NotNull(newTokenObj.Token);
            Assert.NotNull(newTokenObj.RefreshToken);
            Assert.NotEqual(tokenObject.RefreshToken, newTokenObj.RefreshToken);
            Assert.True(DateTime.Now > newTokenObj.Expires);
        }

        [Fact]
        public async Task RevokeTest()
        {
            // # Arrange
            var accoutService = new AccountService(_userManager.Object, _roleManager, _configuration, _logger.Object);
            await accoutService.RevokeAsync("Test");
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(ls[0]);
            mgr.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(ls[0]);
            mgr.Setup(x => x.CheckPasswordAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            mgr.Setup(x => x.GetRolesAsync(It.IsAny<TUser>()))
                .ReturnsAsync(new List<string>() { UserRoles.User });

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>()))
                .ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
        public static Mock<RoleManager<TUser>> MockRoleManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IRoleStore<TUser>>();
            var mgr = new Mock<RoleManager<TUser>>(store.Object, null, null, null, null);

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }
}
