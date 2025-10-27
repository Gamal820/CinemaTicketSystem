using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Services;
using Microsoft.AspNetCore.Identity;
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
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

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
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            // ✅ دعم الـ Areas
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            // ✅ المسار الافتراضي — يدخل على لوحة الإدارة مباشرة
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" }) // ← هنا بنحدد منطقة الـ Admin
                .WithStaticAssets();

            // ✅ أول ما يفتح الموقع "/" → يروح على لوحة الإدارة
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Admin/Dashboard/Index");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}
