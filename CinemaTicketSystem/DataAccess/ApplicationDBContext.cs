using CinemaTicketSystem.DataAccess.EntityConfigurations;
using CinemaTicketSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.DataAccess
{
    public class ApplicationDBContext : DbContext
    {
      
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }

        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieActorEntityTypeConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
