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

        //Navigation Properties
        public List<Recomandation> Recomandations { get; set; }
    }
}
