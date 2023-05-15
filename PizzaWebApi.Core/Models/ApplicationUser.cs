using Microsoft.AspNetCore.Identity;

namespace PizzaWebApi.Core.Models
{
    // По умолчанию Id типа string(varchar(450)), но можно и нужно переопределеить на принятый тип Id для моделей
    // Например для int будет IdentityUser<int>
    // Например для long будет IdentityUser<long>
    public class ApplicationUser : IdentityUser
    {
        // Нужно добавить это поле, если тип Id не string
        //[Key]
        //[PersonalData]
        /////<inheritdoc />
        //public override long Id { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime Expire { get; set; }

        public string Name { get; set; }
    }
}
