using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicketSystem.Models; 
using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Repositories.IRepositories;

namespace CinemaTicketSystem.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {

        private ApplicationDBContext _context;//=new();

        public MovieRepository(ApplicationDBContext context) : base(context) 
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(movies, cancellationToken);
        }
        

    }
}
