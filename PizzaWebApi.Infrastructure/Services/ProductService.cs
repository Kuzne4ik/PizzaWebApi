using MapsterMapper;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.Models;
using PizzaWebApi.SharedKernel.Interfaces;
using PizzaWebApi.Core.ApiModels;
using Microsoft.EntityFrameworkCore;
using Mapster;
using PizzaWebApi.Core;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger? _logger;

        public ProductService(IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IMapper mapper, 
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SearchResult<ProductDTO>> FindAllAsync(SearchCriteriaRequest searchCriteriaRequest)
        {
            _logger.LogInformation($"{nameof(FindAllAsync)} run");

            try
            {
                IQueryable<Product> allQuery;
                if (string.IsNullOrEmpty(searchCriteriaRequest.Keyword))
                    allQuery = _productRepository.ListQuery();
                else
                    allQuery = _productRepository.FindByConditionQuery(t => EF.Functions.Like(t.Name, "%" + searchCriteriaRequest.Keyword + "%"));
                var query = allQuery.Skip(searchCriteriaRequest.Skip).Take(searchCriteriaRequest.PageSize);

                var count = await allQuery.CountAsync();
                var items = await query.ProjectToType<ProductDTO>().ToListAsync();

                return new SearchResult<ProductDTO>(items, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(FindAllAsync)} exception");
                throw new ApplicationException("Get Products failed");
            }
        }

        public async Task<SearchResult<ProductDTO>> GetCategoryProductsAsync(int categoryId, SearchCriteriaRequest searhCriteriaRequest)
        {
            _logger.LogInformation($"{nameof(GetCategoryProductsAsync)} run");
            try
            {
                IQueryable<Product> allQuery = _productRepository
                    .FindByConditionQuery(t => t.CategoryId == categoryId
                        && EF.Functions.Like(t.Name, "%" + searhCriteriaRequest.Keyword + "%"));


                var query = allQuery.Skip(searhCriteriaRequest.Skip).Take(searhCriteriaRequest.PageSize);

                var count = await allQuery.CountAsync();
                var items = await query.ProjectToType<ProductDTO>().ToListAsync();

                return new SearchResult<ProductDTO>(items, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetCategoryProductsAsync)} exception");
                throw new ApplicationException("Get Products by category failed");
            }
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductDTO> GetByIdAsync(int id)
        {
            _logger.LogInformation($"{nameof(GetByIdAsync)} run");
            if (!await ProductIsExistById(id))
                throw new KeyNotFoundException($"The Product {id} not found");

            try
            {
                var product = await _productRepository.FindByConditionQuery(t => t.Id == id).SingleAsync();
                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetByIdAsync)} exception");
                throw new ApplicationException("Get Product failed");
            }
        }

        /// <summary>
        /// Creates a Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns>A newly created Product</returns>
        public async Task<ProductDTO> CreateAsync(ProductDTO productDTO)
        {
            _logger.LogInformation($"{nameof(CreateAsync)} run");

            if (await ProductIsExistByName(productDTO.Name))
                throw new ArgumentException($"A Product with Name {productDTO.Name} already exists.");

            if (await ProductIsExistByTitle(productDTO.Title))
                throw new ArgumentException($"A Product with Title {productDTO.Title} already exists");

            if (!await CategoryIsExists(productDTO.CategoryId))
                throw new KeyNotFoundException($"The Category {productDTO.CategoryId} not found");

            try
            {
                productDTO.Id = 0;
                
                var product = _mapper.Map<Product>(productDTO);
                _ = await _productRepository.AddAsync(product);
                return _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CreateAsync)} exception");
                throw new ApplicationException("Create Product failed");
            }
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ProductDTO productDTO)
        {
            _logger.LogInformation($"{nameof(UpdateAsync)} run");
            if (!await ProductIsExistById(productDTO.Id))
                throw new KeyNotFoundException($"The Product {productDTO.Id} not found");

            if (!await CategoryIsExists(productDTO.CategoryId))
                throw new KeyNotFoundException($"The Category {productDTO.CategoryId} not found");

            try
            {
                var product = _mapper.Map<Product>(productDTO);
                return await _productRepository.UpdateAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateAsync)} exception");
                throw new ApplicationException("Update Product failed");
            }
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Sucess</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"{nameof(DeleteAsync)} run");
            if (!await ProductIsExistById(id))
                throw new KeyNotFoundException($"The Product {id} not found");
            
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                return await _productRepository.DeleteAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteAsync)} exception");
                throw new ApplicationException("Delete Product failed");
            }
        }

        private async Task<bool> ProductIsExistById(int id)
        {
            _logger.LogInformation($"{nameof(ProductIsExistById)} run");
            try
            {
                return await _productRepository.AnyAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductIsExistById)} exception");
                throw new ApplicationException("Get failed");
            }
        }

        private async Task<bool> ProductIsExistByName(string name)
        {
            _logger.LogInformation($"{nameof(ProductIsExistByName)} run");
            try
            {
                return await _productRepository.AnyAsync(t => t.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductIsExistByName)} exception");
                throw new ApplicationException("Get failed");
            }
        }

        private async Task<bool> ProductIsExistByTitle(string title)
        {
            _logger.LogInformation($"{nameof(ProductIsExistByTitle)} run");
            try
            {
                return await _productRepository.AnyAsync(t => t.Title == title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(ProductIsExistByTitle)} exception");
                throw new ApplicationException("Get failed");
            }
        }

        private async Task<bool> CategoryIsExists(int categoryId)
        {
            _logger.LogInformation($"{nameof(CategoryIsExists)} run");
            try
            {
                return await _categoryRepository.AnyAsync(t => t.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(CategoryIsExists)} exception");
                throw new ApplicationException("Get failed");
            }
        }

    }
}
