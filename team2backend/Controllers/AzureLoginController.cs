using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using team2backend.Authentication;

namespace team2backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AzureLoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        //private readonly IConfiguration _configuration;

        public AzureLoginController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult IsUserLoggedIn()
        {
            if (!this.HttpContext.User.Identity.IsAuthenticated)
            {
                return this.Unauthorized();
            }

            return this.Accepted();
        }

        [AllowAnonymous]
        [HttpGet("Authenticate")]
        public async Task Login()
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                ApplicationUser user = new()
                {
                    Email = this.HttpContext.User.Identity.Name.ToString(),
                };
                if (userManager.FindByEmailAsync(user.Email) != null)
                {
                    await userManager.CreateAsync(user);
                }

                if (!await roleManager.RoleExistsAsync("Expert"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Expert"));
                }

                if (await roleManager.RoleExistsAsync("Expert"))
                {
                    await userManager.AddToRoleAsync(user, "Expert");
                }
            }
            else
            {
                this.HttpContext.Items.Add("allowRedirect", true);
                await this.HttpContext.ChallengeAsync();
                return;
            }
        }
    }
}
