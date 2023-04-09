using System.Linq.Expressions;

namespace PizzaWebApi.SharedKernel.Interfaces
{
    public interface IReadRepositoryBase<T> where T : class
    {
        IQueryable<T> ListQuery();

        IQueryable<T> FindByConditionQuery(Expression<Func<T, bool>> expression);

        /// <summary>
        /// With Tracking
        /// </summary>
        /// <typeparam name="TId"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetByIdAsync<TId>(TId id) where TId : notnull;

        Task<IEnumerable<T>> ListAsync();

        Task<IEnumerable<T>> ListByConditionAsync(Expression<Func<T, bool>> expression);

        Task<int> CountAsync();

        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
}
