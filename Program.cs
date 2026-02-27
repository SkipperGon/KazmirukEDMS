using KazmirukEDMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KazmirukEDMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;

            // Configure EF Core + Postgres (connection string in appsettings.json)
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Identity with EF stores
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Lock down defaults for security in an enterprise application
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.SlidingExpiration = true;
                // Redirect unauthenticated users to this path
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                // Do not persist cookie by default - session cookie (user requested behavior)
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
            });

            builder.Services.AddControllersWithViews();
            // Configure Razor Pages and require authorization for document management pages
            builder.Services.AddRazorPages(options =>
            {
                // Protect the Documents folder: unauthenticated users will be redirected to the login page
                options.Conventions.AuthorizeFolder("/Documents");
                // Allow anonymous access to the Account pages (login/logout)
                options.Conventions.AllowAnonymousToPage("/Account/Login");
                options.Conventions.AllowAnonymousToPage("/Account/Logout");
            });
            // Application services
            builder.Services.AddScoped<Services.Interfaces.IDocumentService, Services.DocumentService>();
            // File storage options and service (local storage)
            builder.Services.Configure<Services.LocalStorageOptions>(builder.Configuration.GetSection("LocalStorage"));
            builder.Services.AddSingleton<Services.Interfaces.IFileStorageService, Services.FileStorageService>();

            // Signature service
            builder.Services.Configure<Services.SignatureOptions>(builder.Configuration.GetSection("Signature"));
            builder.Services.AddSingleton<Services.Interfaces.ISignatureService, Services.BouncyCastleSignatureService>();

            var app = builder.Build();

            // Apply pending migrations at startup in development (optional)
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
