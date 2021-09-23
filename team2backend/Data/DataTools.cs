using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using team2backend.Authentication;
using team2backend.Models;

namespace team2backend.Data
{
    public static class DataTools
    {
        // private static UserManager<ApplicationUser> userManager;
        public static async Task SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var appDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                if (appDbContext.Users.Any())
                {
                    Console.WriteLine("The users allready exists!");
                }
                else
                {
                    await EnsureRoleCreated(serviceScope.ServiceProvider, "admin");
                    await EnsureRoleCreated(serviceScope.ServiceProvider, "user");
                    await CreateUserWithRole(app, "AndreiAdmin", "andrei.admin@principal33.com", "TzfOh22_FCbjxXQt6U", "admin");
                    await CreateUserWithRole(app, "AndreiUser", "andrei.user@principal33.com", "jTzPDpuIRyfhGJkjY848", "user");
                    await CreateUserWithRole(app, "AlexUser", "alex.user@principal33.com", "4ewurntQ90gEj4orU6D8", "user");
                    await CreateUserWithRole(app, "LucianUser", "lucian.user@principal33.com", "cmy7baQLWdY5qlOMYZFF", "user");
                }

                // EnsureUsersCreated(services);
                if (appDbContext.Skills.Any())
                {
                    Console.WriteLine("The skills are there!");
                }
                else
                {
                    Skill bE = new ()
                    {
                        Name = "BackEnd",
                    };
                    Skill bEAsp = new ()
                    {
                        Name = "Asp.net",
                        ParentId = 1,
                    };
                    Skill fE = new ()
                    {
                        Name = "FrontEnd",
                    };
                    Skill vue = new ()
                    {
                        Name = "Vue",
                        ParentId = 3,
                    };
                    appDbContext.Skills.Add(bE);
                    appDbContext.Skills.Add(bEAsp);
                    appDbContext.Skills.Add(fE);
                    appDbContext.Skills.Add(vue);
                    appDbContext.SaveChanges();
                }

                if (appDbContext.Recomandations.Any())
                {
                    Console.WriteLine("The recommendations are there!");
                }
                else
                {
                    Recomandation andreiRecommendationForC = new ()
                    {
                        CourseId = 806922,
                        CourseTitle = "The Complete ASP.NET MVC 5 Course",
                        Rating = 4,
                        UserName = "AndreiAdmin",
                        Feedback = "It was ok!",
                        SkillName = "Asp.net",
                    };

                    var skillAsp = appDbContext.Skills
                        .FirstOrDefault(_ => _.Name == andreiRecommendationForC.SkillName);
                    andreiRecommendationForC.SkillId = skillAsp.Id;

                    var userAsp = appDbContext.Users
                         .FirstOrDefault(_ => _.UserName == "AndreiAdmin");

                    // userManager.FindByNameAsync("AndreiAdmin");
                    andreiRecommendationForC.UserId = userAsp.Id;

                    appDbContext.Recomandations.Add(andreiRecommendationForC);
                    appDbContext.SaveChanges();
                }
            }
        }

        public static async Task CreateUserWithRole(IApplicationBuilder app, string username, string email, string password, string role)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                ApplicationUser user = new ()
                {
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = username,
                };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
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

        private static async Task<IdentityUser> EnsureUserCreated(UserManager<IdentityUser> userManager, string name, string password, string email)
        {
            var newUser = await userManager.FindByNameAsync(name);
            if (newUser == null)
            {
                await userManager.CreateAsync(new IdentityUser(name));
                newUser = await userManager.FindByNameAsync(name);
                var tokenChangePassword = await userManager.GeneratePasswordResetTokenAsync(newUser);

                await userManager.ResetPasswordAsync(newUser, tokenChangePassword, password);
                await userManager.SetEmailAsync(newUser, email);
            }

            return newUser;
        }

        private static async Task EnsureUsersCreated(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var andreiAdmin = await EnsureUserCreated(userManager, "AndreiAdmin", "TzfOh22_FCbjxXQt6U", "andrei.admin@principal33.com");
            var andreiUser = await EnsureUserCreated(userManager, "AndreiUser", "jTzPDpuIRyfhGJkjY848", "andrei.user@principal33.com");
            var alexUser = await EnsureUserCreated(userManager, "AlexUser", "4ewurntQ90gEj4orU6D8", "alex.user@principal33.com");
            var lucianUser = await EnsureUserCreated(userManager, "LucianUser", "cmy7baQLWdY5qlOMYZFF", "lucian.user@principal33.com");

            var adminRole = await EnsureRoleCreated(serviceProvider, "Administrator");
            var userRole = await EnsureRoleCreated(serviceProvider, "User");

            await userManager.AddToRoleAsync(andreiAdmin, adminRole.Name);
            await userManager.AddToRoleAsync(andreiUser, userRole.Name);
            await userManager.AddToRoleAsync(alexUser, adminRole.Name);
            await userManager.AddToRoleAsync(lucianUser, userRole.Name);

            var users = await userManager.Users.ToListAsync();
            Console.WriteLine($"There are {users.Count} users now.");
        }
    }
}
