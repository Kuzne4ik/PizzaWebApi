﻿#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Web.Attributes;

namespace PizzaWebApi.Web.Api
{
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoriesController(ICategoryService categoriesService, IProductService productService)
        {
            _categoryService = categoriesService;
            _productService = productService;
        }

        /// <summary>
        /// Get All Categories
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchResult<CategoryDTO>))]
        [HttpPost("search", Name = "SearchCategories")]
        public Task<SearchResult<CategoryDTO>> GetAllAsync([FromBody] SearchCriteriaRequest searchCriteriaRequest)
        {
            return _categoryService.GetAllAsync(searchCriteriaRequest);
        }

        [HttpPost("{categoryId:int}/products/search", Name = "SearchCategoryProducts"), EnsureCategoryExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchResult<ProductDTO>))]
        public Task<SearchResult<ProductDTO>> GetCategoryProductsAsync(int categoryId, [FromBody] SearchCriteriaRequest searhCriteriaRequest)
        {
            return _productService.GetCategoryProductsAsync(categoryId, searhCriteriaRequest);
        }

        /// <summary>
        /// Get Category by Id
        /// </summary>
        /// <param name="id">Category ID</param>
        [HttpGet("{id}", Name = "GetCategoryById"), EnsureCategoryExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<CategoryDTO> GetAsync(int id)
        {
            return _categoryService.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a Category
        /// </summary>
        /// <param name="categoryDTO"></param>
        /// <returns>A newly created Category</returns>
        /// <response code="201">Returns the newly created item</response>
        [HttpPost(Name = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<CategoryDTO> Create([FromBody] CategoryDTO categoryDTO)
        {
            return _categoryService.CreateAsync(categoryDTO);
        }

        /// <summary>
        /// Update Category
        /// </summary>
        /// <param name="categoryDTO"></param>
        [HttpPut(Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> Edit([FromBody] CategoryDTO categoryDTO)
        {
            return _categoryService.UpdateAsync(categoryDTO);
        }

        /// <summary>
        /// Delete Category
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Sucess</returns>
        [HttpDelete("{id:int}", Name = "DeleteCategory"), EnsureCategoryExists]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<bool> Delete(int id)
        {
            return _categoryService.DeleteAsync(id);
        }
    }
}
