using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Models;

namespace team2backend.Dtos
{
    public class GetSkillByIdDto
    {
        public string Name { get; set; }

        public IEnumerable<Recomandation> Recomandations { get; set; }
    }
}
