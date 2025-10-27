using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem
{
    public static class AppConfiguration
    {
        public static void RegisterConfig( this IServiceCollection Services,string connection)
        {
             Services.AddControllersWithViews();
         



             Services.AddDbContext<ApplicationDBContext>(option =>
            {
                option.UseSqlServer(connection);

            });


             Services.AddScoped<DashboardService>();
            Services.AddScoped<IRepository<Category>, Repository<Category>>();
             Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
             Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
             Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
             Services.AddScoped<IMovieActorRepository, MovieActorRepository>();
             Services.AddScoped<IRepository<MovieSubImage>, Repository<MovieSubImage>>();


           Services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();

        }

    }
}
