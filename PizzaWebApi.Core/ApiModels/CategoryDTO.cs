using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// Category Data contract
    /// </summary>
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
