using PizzaWebApi.Core.Models;
using PizzaWebApi.Core.Interfaces;

namespace PizzaWebApi.Infrastructure.Data
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext repositoryContext) : base(repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }
    }
}
