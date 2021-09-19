using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Dtos
{
    public class ReadUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
