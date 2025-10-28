using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Services;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using CinemaTicketSystem.Utitlies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e

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
<<<<<<< HEAD
            Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = false;
                option.SignIn.RequireConfirmedEmail = true;
            })
               .AddEntityFrameworkStores<ApplicationDBContext>()
               .AddDefaultTokenProviders();

            Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Default login path
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Default access denied path
            });

            Services.AddTransient<IEmailSender, EmailSender>();



            Services.AddScoped<DashboardService>();
=======


             Services.AddScoped<DashboardService>();
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e
            Services.AddScoped<IRepository<Category>, Repository<Category>>();
             Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
             Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
             Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
             Services.AddScoped<IMovieActorRepository, MovieActorRepository>();
             Services.AddScoped<IRepository<MovieSubImage>, Repository<MovieSubImage>>();


           Services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
<<<<<<< HEAD
            Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
=======
>>>>>>> c4f5c332a0ae232b974e9fce5ff9335b446aa44e

        }

    }
}
