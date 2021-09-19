using System.Collections.Generic;

namespace team2backend.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Recomandation> Recomandations { get; set; }
    }
}
