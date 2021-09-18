using System.Collections.Generic;
using team2backend.Models;

namespace team2backend.DTOs
{
    public class ReadUserRecommendationsDTO
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public IEnumerable<Recomandation> Recommendations { get; set; }
    }
}
