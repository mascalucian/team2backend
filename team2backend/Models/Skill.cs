using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Data;

namespace team2backend.Models
{
    public class Skill : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Recomandation> Recomandations { get; set; }
    }
}
