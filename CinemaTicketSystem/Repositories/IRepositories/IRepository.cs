using CinemaTicketSystem.Models;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore;
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
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
<<<<<<< HEAD
            Expression<Func<Movie, bool>>? filter = null);
=======
            Expression<Func<Movie, bool>> filter = null);
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e

        Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default);

        Task CommitAsync(CancellationToken cancellationToken = default);
<<<<<<< HEAD

=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        Task AddRangeAsync(List<MovieSubImage> subImagesList, CancellationToken cancellationToken);
        void RemoveRange(List<MovieSubImage> imagesToDelete);
    }
}
