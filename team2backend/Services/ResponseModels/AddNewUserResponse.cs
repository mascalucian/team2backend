using System.Collections.Generic;
using team2backend.Authentication;
using team2backend.Authentication.Models;

namespace team2backend.Services.ResponseModels
{
    public class AddNewUserResponse
    {
        public ApplicationUser User { get; set; }

        public List<string> GoodRoles { get; set; }

        public ResponseAuth Response { get; set; }
    }
}
