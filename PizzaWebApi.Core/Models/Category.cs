using Microsoft.EntityFrameworkCore;
using PizzaWebApi.SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaWebApi.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(Title), IsUnique = true)]
    public class Category : BaseEntity
    {
        [Column(TypeName = "nvarchar(250)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        public List<Product> Products { get; set; }
    }
}