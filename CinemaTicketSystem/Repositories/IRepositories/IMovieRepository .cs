using CinemaTicketSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Repositories.IRepositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
         Task AddRangeAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken = default);
      
    }
}
