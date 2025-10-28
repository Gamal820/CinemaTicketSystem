using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaTicketSystem.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
<<<<<<< HEAD
        private readonly ApplicationDBContext _context;
        private readonly DbSet<T> _dbSet;
=======
        // 🛑 ملاحظة: هذا النمط يفضل تغييره إلى حقن التبعية
        private ApplicationDBContext _context;// = new();
        private DbSet<T> _dbSet;
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e

        public Repository(ApplicationDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

<<<<<<< HEAD
        // ✅ إضافة كائن جديد
=======
        // CRUD

>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _dbSet.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

<<<<<<< HEAD
        // ✅ تعديل كائن
=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        public void Update(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Update(entity);
        }
<<<<<<< HEAD

        // ✅ حذف كائن
=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

<<<<<<< HEAD
        // ✅ جلب مجموعة بيانات
=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
<<<<<<< HEAD
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
=======
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
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
<<<<<<< HEAD
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
=======
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
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
