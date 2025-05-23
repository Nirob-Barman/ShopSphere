
namespace ShopSphere.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        void BeginTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }

}
