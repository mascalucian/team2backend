using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using team2backend.Authentication;
using team2backend.Dtos;
using team2backend.Models;
using team2backend.Services.ResponseModels;

namespace team2backend.Interfaces
{
    public interface IUsersRepository
    {
        Task<AddNewUserResponse> AddNewUserWithRoles([FromBody] AddNewUser user);

        Task<ApplicationUser> EidtRolesForUser(string id, [FromBody] string[] roles);

        Task<ReadUserDto> DeleteUser(string id);

        Task<ReadUserDto> GetUserById(string id);

        Task<List<ReadUserDto>> GetUsers();
    }
}