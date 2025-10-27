using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CinemaTicketSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
           

            // Add services to the container.
            builder.Services.AddControllersWithViews();

          
            var connectionString =
               builder.Configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string"
                   + "'DefaultConnection' not found.");


            builder.Services.RegisterConfig(connectionString);





            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapStaticAssets();

            // Routes for Areas
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // Default route (Admin area)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" })
                .WithStaticAssets();

            app.Run();
        }
    }
}
