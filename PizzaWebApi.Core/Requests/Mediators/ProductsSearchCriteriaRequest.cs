using MediatR;
using PizzaWebApi.Core.ApiModels;
using PizzaWebApi.Core.Response;

namespace PizzaWebApi.Core.Requests.Mediators;

public class ProductsSearchCriteriaRequest : SearchCriteriaRequest, IRequest<SearchResult<ProductDTO>>
{

}
