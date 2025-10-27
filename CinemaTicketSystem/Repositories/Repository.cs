using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaTicketSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        // 🛑 ملاحظة: هذا النمط يفضل تغييره إلى حقن التبعية
        private ApplicationDBContext _context;// = new();
        private DbSet<T> _dbSet;

        public Repository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // CRUD

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public void Update(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            var entities = _dbSet.AsQueryable();

            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not null)
            {
                foreach (var item in includes)
                    entities = entities.Include(item);
            }

            if (!tracked)
                entities = entities.AsNoTracking();

            //entities = entities.Where(e => e.Status);

            return await entities.ToListAsync(cancellationToken);
        }

        // 💡 تم تحسين هذه الدالة لعدم جلب جميع السجلات قبل اختيار سجل واحد.
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            var entities = _dbSet.AsQueryable();

            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not null)
            {
                foreach (var item in includes)
                    entities = entities.Include(item);
            }

            if (!tracked)
                entities = entities.AsNoTracking();

            // استخدام FirstOrDefaultAsync مباشرة على الـ IQueryable
            return await entities.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public Task AddRangeAsync(List<MovieSubImage> subImagesList, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(List<MovieSubImage> imagesToDelete)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true, CancellationToken cancellationToken = default, Expression<Func<Movie, bool>> filter = null)
        {
            throw new NotImplementedException();
        }
    }
}