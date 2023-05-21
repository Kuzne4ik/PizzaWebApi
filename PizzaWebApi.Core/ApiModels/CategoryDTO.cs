using System.ComponentModel.DataAnnotations;

namespace PizzaWebApi.Core.ApiModels
{
    /// <summary>
    /// Category Data contract
    /// </summary>
    /// <remarks>
    /// Валидатор <see cref="Validators.CategoryDTOValidator"/>
    /// </remarks>
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
