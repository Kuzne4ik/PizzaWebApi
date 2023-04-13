using Microsoft.EntityFrameworkCore;
using PizzaWebApi.SharedKernel.Interfaces;
using System.Linq.Expressions;


namespace PizzaWebApi.Infrastructure.Data
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDbContext RepositoryContext { get; set; }
        public RepositoryBase(AppDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public IQueryable<T> ListQuery() => RepositoryContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByConditionQuery(Expression<Func<T, bool>> expression) => RepositoryContext.Set<T>().Where(expression).AsNoTracking();

        /// <summary>
        /// With Tracking
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T?> FindByIdAsync<TId>(TId id) where TId : notnull => await RepositoryContext.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> ListAsync() => await RepositoryContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<T>> ListByConditionAsync(Expression<Func<T, bool>> expression) => await FindByConditionQuery(expression).ToListAsync();

        public async Task<int> CountAsync() => await RepositoryContext.Set<T>().AsNoTracking().CountAsync();

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression) => await RepositoryContext.Set<T>().AnyAsync(expression);

        public async Task<bool> AddAsync(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
            var res = await RepositoryContext.SaveChangesAsync();
            return res > 0;
        }
        public async Task<bool> UpdateAsync(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
            var res = await RepositoryContext.SaveChangesAsync();
            return res > 0;
        }
        public async Task<bool> DeleteAsync(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
            var res = await RepositoryContext.SaveChangesAsync();
            return res > 0;
        }
    }

}
