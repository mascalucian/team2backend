using System.Collections.Generic;

namespace team2backend.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<Recomandation> Recommendations { get; set; }
    }
}
