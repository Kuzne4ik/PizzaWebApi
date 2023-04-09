using Microsoft.AspNetCore.Identity;

namespace PizzaWebApi.Core.Models
{
    // Правильно было бы так : IdentityUser<int> по дефолту varchar(450)
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime Expire { get; set; }

        public string Name { get; set; }
    }
}
