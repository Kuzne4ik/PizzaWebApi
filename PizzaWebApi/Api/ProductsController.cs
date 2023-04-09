#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using MediatR;
using PizzaWebApi.Core.Requests.Mediators;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Core.Requests;

namespace PizzaWebApi.Web.Api
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productsService;
        private IMediator _mediator;

        public ProductsController(IProductService productsService, IMediator mediator)
        {
            _productsService = productsService;
            _mediator = mediator;
        }

        /// <summary>
        /// Get All Products
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SearchResult<ProductDTO>))]
        [HttpPost("search", Name = "SearchProducts") ]
        public Task<SearchResult<ProductDTO>> GetAllAsync([FromBody] SearchCriteriaRequest searhCriteriaRequest)
        {
            return _mediator.Send(new ProductsSearchCriteriaRequest { 
                Keyword = searhCriteriaRequest.Keyword, Page = searhCriteriaRequest.Page, PageSize = searhCriteriaRequest.PageSize 
            });
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <param name="id">Product ID</param>
        [HttpGet("{id}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ProductDTO> GetAsync(int id)
        {
            return _productsService.GetByIdAsync(id);
        }

        /// <summary>
        /// Creates a Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns>A newly created Product</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <exception cref="DbUpdateException"></exception>
        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<ProductDTO> Create([FromBody] ProductDTO productDTO)
        {
            return _productsService.CreateAsync(productDTO);
        }

        /// <summary>
        /// Update Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        [HttpPut("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> Edit([FromBody] ProductDTO productDTO)
        {
            return _productsService.UpdateAsync(productDTO);
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public Task<bool> Delete(int id)
        {
            return _productsService.DeleteAsync(id);
        }
    }
}
