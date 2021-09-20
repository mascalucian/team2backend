using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using team2backend.Authentication;
using team2backend.Authentication.Models;
using team2backend.Dtos;
using team2backend.Interfaces;

namespace team2backend.Services
{
    public class DbAuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public DbAuthRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        public object Login(LoginModel model)
        {
            //var user = userManager.FindByNameAsync(model.Username);
            //if (user != null && userManager.CheckPasswordAsync(user, model.Password))
            //{
            //    var userRoles = userManager.GetRolesAsync(user);

            //    var authClaims = new List<Claim>
            //    {
            //        new Claim(ClaimTypes.Name, user.UserName),
            //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //    };

            //    foreach (var userRole in userRoles)
            //    {
            //        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            //    }

            //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            //    var token = new JwtSecurityToken(
            //        issuer: _configuration["JWT:ValidIssuer"],
            //        audience: _configuration["JWT:ValidAudience"],
            //        expires: DateTime.Now.AddHours(3),
            //        claims: authClaims,
            //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            //    var readUserDto = new ReadUserDto
            //    {
            //        Id = user.Id,
            //        UserName = user.UserName,
            //        Email = user.Email,
            //        Roles = userManager.GetRolesAsync(user).ConfigureAwait(false)
            //    };
            //    return new
            //    {
            //        token = new JwtSecurityTokenHandler().WriteToken(token),
            //        expiration = token.ValidTo,
            //        user = readUserDto,
            //    };
            //}

            return new { };
        }

        public ResponseAuth Register(RegisterModel model)
        {
            throw new NotImplementedException();
        }

        public ResponseAuth RegisterAdmin(RegisterModel model)
        {
            throw new NotImplementedException();
        }
    }
}
