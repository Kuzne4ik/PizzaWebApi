namespace PizzaWebApi.SharedKernel.Interfaces
{
    public interface IRepositoryBase<T> : IReadRepositoryBase<T> where T : class//, IAggregateRoot
    {
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }
}
