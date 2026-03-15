using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KazmirukEDMS.Data
{
    /// <summary>
    /// Database initializer / seeder.
    /// Creates required roles and an administrator user when missing.
    /// Credentials for the initial admin are read from environment variables:
    /// - EDMS_ADMIN_USERNAME
    /// - EDMS_ADMIN_PASSWORD
    /// Optionally EDMS_ADMIN_EMAIL
    /// If username or password are not provided, seeding of admin user is skipped.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, ILogger? logger = null)
        {
            using var scope = services.CreateScope();
            var svc = scope.ServiceProvider;

            var userManager = svc.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = svc.GetRequiredService<RoleManager<IdentityRole>>();

            logger ??= svc.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer");

            const string adminRole = "Administrator";

            // Ensure Administrator role exists
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var r = new IdentityRole(adminRole);
                var rr = await roleManager.CreateAsync(r);
                if (rr.Succeeded)
                {
                    logger?.LogInformation("Created role '{Role}'", adminRole);
                }
                else
                {
                    logger?.LogWarning("Failed to create role '{Role}': {Errors}", adminRole, string.Join(',', rr.Errors.Select(e => e.Description)));
                }
            }

            // Read admin credentials from environment
            var adminUserName = Environment.GetEnvironmentVariable("EDMS_ADMIN_USERNAME");
            var adminPassword = Environment.GetEnvironmentVariable("EDMS_ADMIN_PASSWORD");
            var adminEmail = Environment.GetEnvironmentVariable("EDMS_ADMIN_EMAIL");

            if (string.IsNullOrWhiteSpace(adminUserName) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger?.LogWarning("Environment variables EDMS_ADMIN_USERNAME and/or EDMS_ADMIN_PASSWORD not set. Skipping admin user creation.");
                return;
            }

            var existing = await userManager.FindByNameAsync(adminUserName);
            if (existing != null)
            {
                logger?.LogInformation("Admin user '{User}' already exists.", adminUserName);
                // Ensure user is in Administrator role
                if (!await userManager.IsInRoleAsync(existing, adminRole))
                {
                    var add = await userManager.AddToRoleAsync(existing, adminRole);
                    if (add.Succeeded) logger?.LogInformation("Added existing admin to '{Role}' role.", adminRole);
                }
                return;
            }

            var admin = new IdentityUser
            {
                UserName = adminUserName,
                Email = string.IsNullOrWhiteSpace(adminEmail) ? adminUserName : adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                logger?.LogError("Failed to create admin user: {Errors}", string.Join(',', createResult.Errors.Select(e => e.Description)));
                return;
            }

            var roleResult = await userManager.AddToRoleAsync(admin, adminRole);
            if (!roleResult.Succeeded)
            {
                logger?.LogWarning("Admin user created but failed to add to role '{Role}': {Errors}", adminRole, string.Join(',', roleResult.Errors.Select(e => e.Description)));
            }
            else
            {
                logger?.LogInformation("Admin user '{User}' created and added to '{Role}' role.", adminUserName, adminRole);
            }
        }
    }
}
