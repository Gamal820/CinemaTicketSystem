using CinemaTicketSystem.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ? √÷› Â–« «·”ÿ— · ”ÃÌ· «·‹ DbContext
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer("Data Source=DESKTOP-90TMC45\\MSSQLSERVER2;Initial Catalog=CinemaTicketSystem;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"));

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
