using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LarmoireWeb.Data;
using LarmoireWeb.Data.Services;
using LarmoireWeb.Models;

namespace LarmoireWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) DbContext (SQL Server docker)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // :contentReference[oaicite:0]{index=0}&#8203;:contentReference[oaicite:1]{index=1}

            // 2) Identity avec Entity Framework
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();  // :contentReference[oaicite:2]{index=2}&#8203;:contentReference[oaicite:3]{index=3}

            // 3) Configuration du cookie d’authentification
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            // 4) Injection de vos services métier
            builder.Services.AddScoped<IProduitService, ProduitService>();
            builder.Services.AddScoped<ICategorieService, CategorieService>();
            builder.Services.AddScoped<IPanierService, PanierService>();
            builder.Services.AddScoped<ICommandeService, CommandeService>();
            builder.Services.AddScoped<IRenovationService, RenovationService>();
            builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();

            // 5) MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 6) Pipeline HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();   // <— Important pour Identity
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
