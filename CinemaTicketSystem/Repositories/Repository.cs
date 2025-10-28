using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaTicketSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // ✅ إضافة كائن جديد
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        // ✅ تعديل كائن
        public void Update(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Update(entity);
        }

        // ✅ حذف كائن
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // ✅ جلب مجموعة بيانات
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default,
            Expression<Func<Movie, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (expression is not null)
                query = query.Where(expression);

            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (!tracked)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

        // ✅ جلب سجل واحد فقط
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (expression is not null)
                query = query.Where(expression);

            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        // ✅ حفظ التغييرات
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ✅ إضافة مجموعة SubImages
        public async Task AddRangeAsync(List<MovieSubImage> subImagesList, CancellationToken cancellationToken)
        {
            await _context.MovieSubImages.AddRangeAsync(subImagesList, cancellationToken);
        }

        // ✅ حذف مجموعة SubImages
        public void RemoveRange(List<MovieSubImage> imagesToDelete)
        {
            _context.MovieSubImages.RemoveRange(imagesToDelete);
        }
    }
}
