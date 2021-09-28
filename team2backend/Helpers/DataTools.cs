using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using team2backend.Authentication;

namespace team2backend.Helpers
{
    public static class DataTools
    {
        public static async Task SeedData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                var adminRole = await EnsureRoleCreated(serviceScope.ServiceProvider, UserRoles.Admin);
                var operatorRole = await EnsureRoleCreated(serviceScope.ServiceProvider, UserRoles.Expert);
                var userRole = await EnsureRoleCreated(serviceScope.ServiceProvider, UserRoles.User);

                ApplicationUser andreiAdmin = new ()
                {
                    Email = "andreidirlea.97@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "andreidirlea.97@gmail.com",
                };
                ApplicationUser andreiUser = new ()
                {
                    Email = "andreidirleaUser.97@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "andreidirleaUser.97@gmail.com",
                };
                if (await userManager.FindByEmailAsync(andreiAdmin.Email) == null)
                {
                    await userManager.CreateAsync(andreiAdmin, "Pass123$");
                    await userManager.AddToRoleAsync(andreiAdmin, UserRoles.Admin);
                }

                if (await userManager.FindByEmailAsync(andreiUser.Email) == null)
                {
                    await userManager.CreateAsync(andreiUser, "Pass123$");
                    await userManager.AddToRoleAsync(andreiUser, UserRoles.User);
                }
            }
        }

        private static async Task<IdentityRole> EnsureRoleCreated(IServiceProvider serviceProvider, string roleName)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var newRole = await roleManager.FindByNameAsync(roleName);
            if (newRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                newRole = await roleManager.FindByNameAsync(roleName);
            }

            return newRole;
        }
    }
}
