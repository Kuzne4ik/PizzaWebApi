using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Core.Interfaces
{
    public interface IProductService
    {
        Task<SearchResult<ProductDTO>> FindAllAsync(SearchCriteriaRequest searchCriteriaRequest);

        Task<SearchResult<ProductDTO>> GetCategoryProductsAsync(int categoryId, SearchCriteriaRequest searhCriteriaRequest);

        Task<ProductDTO> GetByIdAsync(int id);

        Task<ProductDTO> CreateAsync(ProductDTO category);

        Task<bool> UpdateAsync(ProductDTO category);

        Task<bool> DeleteAsync(int id);
    }
}
