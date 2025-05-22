
using ShopSphere.Application.Interfaces.Persistence;

namespace ShopSphere.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        //public IRepository<T> Repository<T>() where T : class
        //{
        //    var type = typeof(T);

        //    if (!_repositories.ContainsKey(type))
        //    {
        //        var repositoryInstance = new GenericRepository<T>(_context);
        //        _repositories.Add(type, repositoryInstance);
        //    }

        //    return (IRepository<T>)_repositories[type];
        //}

        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (_repositories.ContainsKey(type))
                return (IRepository<T>)_repositories[type];

            var repoInstance = new GenericRepository<T>(_context);
            _repositories[type] = repoInstance;
            return repoInstance;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
