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

                var adminRole = await EnsureRoleCreated(serviceScope.ServiceProvider, "Administrator");
                var operatorRole = await EnsureRoleCreated(serviceScope.ServiceProvider, "Expert");
                var userRole = await EnsureRoleCreated(serviceScope.ServiceProvider, "User");

                ApplicationUser andreiAdmin = new ()
                {
                    Email = "andreidirlea.97@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "AndreiAdmin",
                };
                ApplicationUser andreiUser = new ()
                {
                    Email = "andreidirlea1.97@gmail.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "AndreiUser",
                };

                await userManager.CreateAsync(andreiAdmin, "Pass123$");
                await userManager.AddToRoleAsync(andreiAdmin, "Administrator");

                await userManager.CreateAsync(andreiUser, "Pass123$");
                await userManager.AddToRoleAsync(andreiUser, "User");

                //string[] teamMembers = { "alex", "luci", "andrei", "ovidiu", "dorin" };

                //if (!await roleManager.RoleExistsAsync(UserRoles.User))
                //    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                //    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                //foreach (var _ in teamMembers)
                //{
                //    var userName = "admin_" + _ + "@team2backend.ro";
                //    var userExists = await userManager.FindByNameAsync(userName);
                //    if (userExists == null)
                //    {
                //        ApplicationUser user = new ApplicationUser()
                //        {
                //            Email = "admin_" + _ + "@team2backend.ro",
                //            SecurityStamp = Guid.NewGuid().ToString(),
                //            UserName = "admin_" + _ + "@team2backend.ro",
                //        };
                //        await userManager.CreateAsync(user, "Pass123$");
                //        await userManager.AddToRoleAsync(user, UserRoles.Admin);
                //    }
                //}
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
