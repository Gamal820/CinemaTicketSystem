using System.Collections.Generic;
using CinemaTicketSystem.Models;  
using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Repositories.IRepositories;

namespace CinemaTicketSystem.Repositories
{
    public class MovieActorRepository : Repository<MovieActor>, IMovieActorRepository
    {
        private ApplicationDBContext _context; //= new();

        public MovieActorRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public Task AddRangeAsync(List<MovieActor> newMovieActors, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<MovieActor> movieActors)
        {
            _context.RemoveRange(movieActors);
        }
       
    }
}

