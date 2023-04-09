using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Requests.Mediators;
using PizzaWebApi.Core.Response;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Mediators.Handlers
{
    public class GetAllProductsHandler : IRequestHandler<ProductsSearchCriteriaRequest, SearchResult<ProductDTO>>
    {
        private IProductRepository _productRepository;
        private IMapper _mapper;
        private ILogger<GetAllProductsHandler> _logger;

        public GetAllProductsHandler(
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<GetAllProductsHandler> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SearchResult<ProductDTO>> Handle(ProductsSearchCriteriaRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Run handle {nameof(GetAllProductsHandler)} run");

            try
            {
                IQueryable<Product> allQuery;
                if (string.IsNullOrEmpty(request.Keyword))
                    allQuery = _productRepository.ListQuery();
                else
                    allQuery = _productRepository.FindByConditionQuery(t => EF.Functions.Like(t.Name, "%" + request.Keyword + "%"));
                var query = allQuery.Skip(request.Skip).Take(request.PageSize);

                var count = await allQuery.CountAsync();
                var list = await query.ProjectToType<ProductDTO>().ToListAsync();
                

                var productsSearchResult = new SearchResult<ProductDTO>(list, count);
                return productsSearchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Handle)} exception");
                throw new ApplicationException("Get Products failed");
            }
        }
    }
}
