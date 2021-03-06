using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public IEnumerable<Recomandation> Recomandations { get; set; }
    }
}
