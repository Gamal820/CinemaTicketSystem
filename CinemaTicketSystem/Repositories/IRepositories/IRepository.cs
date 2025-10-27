using CinemaTicketSystem.Models;
using System.Linq.Expressions;

namespace CinemaTicketSystem.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity, CancellationToken cancellationToken);
        void Delete(T entity);

        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default,
            Expression<Func<Movie, bool>>? filter = null);

        Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);

        Task AddRangeAsync(List<MovieSubImage> subImagesList, CancellationToken cancellationToken);
        void RemoveRange(List<MovieSubImage> imagesToDelete);
    }
}
