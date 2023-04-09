using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<SearchResult<CategoryDTO>> GetAllAsync(SearchCriteriaRequest searchCriteriaRequest);

        Task<CategoryDTO> GetByIdAsync(int id);

        Task<CategoryDTO> CreateAsync(CategoryDTO category);

        Task<bool> UpdateAsync(CategoryDTO category);

        Task<bool> DeleteAsync(int id);
        
    }
}
