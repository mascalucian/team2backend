using team2backend.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using team2backend.Authentication;
using IdentityServer4.Services;
using IdentityServer4.Models;
using IdentityModel;

namespace team2backend.Services
{
    public class ProfileService : IProfileService
    {
        protected UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
                new Claim(JwtClaimTypes.Name, user.UserName),
        };
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }
            }
            else
            {
                claims.Add(new Claim(JwtClaimTypes.Role, "User"));
            }

            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
            context.IsActive = (user != null) && user.LockoutEnabled;
        }
    }
}
