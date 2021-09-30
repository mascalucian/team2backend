using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using team2backend.Authentication;
using team2backend.Authentication.Models;
using team2backend.Data;
using team2backend.Dtos;
using team2backend.Interfaces;
using team2backend.Models;
using team2backend.Services.ResponseModels;

namespace team2backend.Services
{
    public class DbUsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;

        public DbUsersRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
        }

        public async Task<AddNewUserResponse> AddNewUserWithRoles([FromBody] AddNewUser user)
        {
            var userExists = await userManager.FindByEmailAsync(user.Email);
            if (userExists != null)
            {
                return new AddNewUserResponse
                {
                    User = null,
                    Response = new () { Status = "Error", Message = "User already exists!" },
                };
            }

            ApplicationUser newUser = new ()
            {
                Email = user.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = user.Email,
            };

            var roles = await CheckRoles(user.Roles);

            var result = await userManager.CreateAsync(newUser, user.Password);
            if (!result.Succeeded)
            {
                return new AddNewUserResponse
                {
                    User = null,
                    Response = new () { Status = "Error", Message = "User creation failed! Please check user details and try again ." },
                };
            }

            var badRolesInString = string.Empty;
            foreach (var badRole in roles.BadRoles)
            {
                badRolesInString = badRolesInString + " " + badRole;
            }

            if (roles.GoodRoles.Count > 0)
            {
                foreach (var role in roles.GoodRoles)
                {
                    if (!await userManager.IsInRoleAsync(newUser, role))
                    {
                        await userManager.AddToRoleAsync(newUser, role);
                    }
                }
            }
            else
            {
                await userManager.AddToRoleAsync(newUser, UserRoles.User);
                roles.GoodRoles.Add(UserRoles.User);
            }

            if (roles.BadRoles.Count == 0)
            {
                return new AddNewUserResponse
                {
                    User = newUser,
                    GoodRoles = roles.GoodRoles,
                    Response = new () { Status = "Success", Message = "User created successfully!" },
                };
            }
            else
            {
                return new AddNewUserResponse
                {
                    User = newUser,
                    GoodRoles = roles.GoodRoles,
                    Response = new () { Status = "Success", Message = $"User created successfully, but roles {badRolesInString} are not valid!" },
                };
            }
        }

        private async Task<Roles> CheckRoles(IEnumerable<string> roles)
        {
            var badRoles = new List<string>();
            var goodRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    badRoles.Add(role);
                }
                else
                {
                    goodRoles.Add(role);
                }
            }

            return new Roles
            {
                GoodRoles = goodRoles,
                BadRoles = badRoles,
            };
        }

        public async Task<ReadUserDto> DeleteUser (string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                context.Recomandations.RemoveRange(context.Recomandations.Where(_ => _.UserId == id));
                await context.SaveChangesAsync();
                await userManager.DeleteAsync(user);
                var userDto = mapper.Map<ReadUserDto>(user);
                return userDto;
            }

            return null;
        }

        public async Task<ApplicationUser> EidtRolesForUser(string id, [FromBody] string[] roles)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                await userManager.RemoveFromRolesAsync(user, userRoles.ToArray());
                var checkedRoles = await CheckRoles(roles);
                foreach (var role in checkedRoles.GoodRoles)
                {
                    await userManager.AddToRoleAsync(user, role);
                }

                return user;
            }

            return null;
        }

        public async Task<ReadUserDto> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userDto = mapper.Map<ApplicationUser, ReadUserDto>(user);
                var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                userDto.Roles = roles;
                return userDto;
            }

            return null;
        }

        public async Task<List<ReadUserDto>> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var usersDto = mapper.Map<List<ApplicationUser>, List<ReadUserDto>>(users);
            foreach (var user in usersDto)
            {
                var currentUser = await userManager.FindByIdAsync(user.Id);
                var userRoles = await userManager.GetRolesAsync(currentUser);
                user.Roles = userRoles;
            }

            return usersDto;
        }
    }
}
