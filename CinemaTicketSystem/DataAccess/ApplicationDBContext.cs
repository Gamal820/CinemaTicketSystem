using CinemaTicketSystem.DataAccess.EntityConfigurations;
using CinemaTicketSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CinemaTicketSystem.ViewModels;
namespace CinemaTicketSystem.DataAccess
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<ApplicationUserOTP> ApplicationUserOTP { get; set; }



        //public ApplicationDBContext()
        //{ }
        //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-90TMC45\\MSSQLSERVER2;Initial Catalog=CinemaTicketSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieActorEntityTypeConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<CinemaTicketSystem.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
    }
}
