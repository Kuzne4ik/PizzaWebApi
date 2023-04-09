using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Data
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
    }
}
