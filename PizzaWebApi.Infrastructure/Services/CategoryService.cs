using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger? _logger;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SearchResult<CategoryDTO>> GetAllAsync(SearchCriteriaRequest searchCriteriaRequest)
        {
            _logger.LogInformation($"{nameof(GetAllAsync)} run");
            try
            {
                IQueryable<Category> allQuery;
                if (string.IsNullOrEmpty(searchCriteriaRequest.Keyword))
                    allQuery = _categoryRepository.ListQuery();
                else
                    allQuery = _categoryRepository.FindByConditionQuery(t => EF.Functions.Like(t.Name, "%" + searchCriteriaRequest.Keyword + "%"));
                
                var query = allQuery.Skip(searchCriteriaRequest.Skip).Take(searchCriteriaRequest.PageSize);

                var count = await allQuery.CountAsync();
                var items = await query.ProjectToType<CategoryDTO>().ToListAsync();

                return new SearchResult<CategoryDTO>(items, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetAllAsync)} exception");
                throw new ApplicationException("Get Categories failed");
            }
        }

        /// <summary>
        /// Get Category by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<CategoryDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} run");
            try
            {
                var category = await _categoryRepository.FindByConditionQuery(t => t.Id == id).SingleAsync();
                return category.Adapt<CategoryDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetByIdAsync)} exception");
                throw new ApplicationException("Get Category failed");
            }
        }

        /// <summary>
        /// Creates a Category
        /// </summary>
        /// <param name="categoryDTO"></param>
        /// <returns>A newly created Category</returns>
        public async Task<CategoryDTO> CreateAsync(CategoryDTO categoryDTO)
        {
            _logger.LogInformation($"{nameof(CreateAsync)} run");
            if (await CategoryIsExistByName(categoryDTO.Name))
            {
                throw new Exception($"A Category with Name {categoryDTO.Name} already exists.");
            }
            if (await CategoryIsExistByTitle(categoryDTO.Title))
            {
                throw new Exception($"A Category with Title {categoryDTO.Title} already exists.");
            }
            try
            {
                categoryDTO.Id = 0;
                var category = _mapper.Map<Category>(categoryDTO);
                _ = await _categoryRepository.AddAsync(category);
                return category.Adapt(categoryDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CreateAsync)} exception");
                throw new ApplicationException("Create Category failed");
            }
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="categoryDTO"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(CategoryDTO categoryDTO)
        {
            _logger.LogInformation($"{nameof(UpdateAsync)} run");
            try
            {
                return await _categoryRepository.UpdateAsync(categoryDTO.Adapt<Category>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateAsync)} exception");
                throw new ApplicationException("Update Category failed");
            }
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Sucess</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(DeleteAsync)} run");
            
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                return await _categoryRepository.DeleteAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteAsync)} exception");
                throw new ApplicationException("Delete Category failed");
            }
        }

        public async Task<bool> CategoryIsExistById(int id)
        {
            _logger.LogInformation($"{nameof(CategoryIsExistById)} run");
            try
            {
                return await _categoryRepository.AnyAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryIsExistById)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        private async Task<bool> CategoryIsExistByName(string name)
        {
            _logger.LogInformation($"{nameof(CategoryIsExistByName)} run");
            try
            {
                return await _categoryRepository.AnyAsync(t => t.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryIsExistByName)} exception");
                throw new ApplicationException("Any failed");
            }
        }

        private async Task<bool> CategoryIsExistByTitle(string title)
        {
            _logger.LogInformation($"{nameof(CategoryIsExistByTitle)} run");
            try
            {
                return await _categoryRepository.AnyAsync(t => t.Title == title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryIsExistByTitle)} exception");
                throw new ApplicationException("Any failed");
            }
        }
    }
}
