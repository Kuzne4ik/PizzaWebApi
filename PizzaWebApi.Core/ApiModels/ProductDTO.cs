using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// Product Data contract
    /// </summary>
    public class ProductDTO
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
    }
}
