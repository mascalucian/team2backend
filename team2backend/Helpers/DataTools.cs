using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

                string[] teamMembers = { "alex", "luci", "andrei", "ovidiu", "dorin" };

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

                foreach (var _ in teamMembers)
                {
                    var userName = "admin_" + _ + "@team2backend.ro";
                    var userExists = await userManager.FindByNameAsync(userName);
                    if (userExists == null)
                    {
                        ApplicationUser user = new ApplicationUser()
                        {
                            Email = "admin_" + _ + "@team2backend.ro",
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserName = "admin_" + _ + "@team2backend.ro",
                        };
                        await userManager.CreateAsync(user, "Pass123$");
                        await userManager.AddToRoleAsync(user, UserRoles.Admin);
                    }
                }
            }
        }
    }
}
