using CinemaTicketSystem.Configurations;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Utitlies.DBInitilizer;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Microsoft.AspNetCore.Http;
using CinemaTicketSystem.Utitlies;

namespace CinemaTicketSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.RegisterMapsterConfig();
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.RegisterConfig(connectionString);

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            // ✅ التعامل الصحيح مع الكوكيز لمنع خطأ oauth state missing
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            // ✅ External Login With Google (الصحيح)
            builder.Services.AddAuthentication()
                .AddGoogle(opt =>
                {
                    var googleAuth = builder.Configuration.GetSection("Authentication:Google");
                    opt.ClientId = googleAuth["ClientId"] ?? "";
                    opt.ClientSecret = googleAuth["ClientSecret"] ?? "";
                    opt.CallbackPath = "/signin-google"; // مهم جداً
                });

            var app = builder.Build();

            // ✅ تشغيل DBInitializer
            using (var scope = app.Services.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                service.Initialize();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // ✅ مهم

            app.UseRouting();

            app.UseAuthentication(); // ✅ لازم قبل Authorization
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });

            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Admin/Dashboard/Index");
                return Task.CompletedTask;
            });

            app.Run();
        }
    }
}
