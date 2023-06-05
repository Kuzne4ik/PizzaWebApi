#nullable disable
using Microsoft.AspNetCore.Mvc;
using PizzaWebApi.Core.Interfaces;
using PizzaWebApi.Core.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using PizzaWebApi.Core.Requests.Mediators;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Core.Requests;
using PizzaWebApi.Web.Attributes;

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
        public Task<SearchResult<ProductDTO>> GetAllAsync([FromBody] SearchCriteriaRequest searchCriteriaRequest)
        {
            return _mediator.Send(new ProductsSearchCriteriaRequest { 
                Keyword = searchCriteriaRequest.Keyword, Page = searchCriteriaRequest.Page, PageSize = searchCriteriaRequest.PageSize 
            });
        }

        /// <summary>
        /// Get Product by Id
        /// </summary>
        /// <param name="productId">Product ID</param>
        [HttpGet("{productId}", Name = "GetProductById"), EnsureProductExists]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ProductDTO> GetAsync(int productId)
        {
            return _productsService.GetByIdAsync(productId);
        }

        /// <summary>
        /// Creates a Product
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns>A newly created Product</returns>
        /// <response code="201">Returns the newly created item</response>
        [Authorize]
        [HttpPost(Name = "CreateProduct")]
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
        [Authorize]
        [HttpPut("UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task<bool> Edit([FromBody] ProductDTO productDTO)
        {
            return _productsService.UpdateAsync(productDTO);
        }

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="productId">Product ID</param>
        [Authorize]
        [HttpDelete("{productId:int}", Name = "DeleteProduct"), EnsureProductExists]
        
        public Task<bool> Delete(int productId)
        {
            return _productsService.DeleteAsync(productId);
        }
    }
}
