using PizzaWebApi.SharedKernel.Interfaces;

namespace PizzaWebApi.Infrastructure.Data
{
    public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class
    {
        public EfRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
