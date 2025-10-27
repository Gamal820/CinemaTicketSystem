using CinemaTicketSystem.Models;

namespace CinemaTicketSystem.Repositories.IRepositories
{
    public interface IMovieActorRepository : IRepository<MovieActor>
    {
        Task AddRangeAsync(List<MovieActor> newMovieActors, CancellationToken cancellationToken);
        void RemoveRange(IEnumerable<MovieActor> movieActors);
    }
}
