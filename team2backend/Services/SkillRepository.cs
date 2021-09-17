using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Data;
using team2backend.Data.EfCore;
using team2backend.Models;

namespace team2backend.Services
{
    public class SkillRepository : EfCoreRepository<Skill, ApplicationDbContext>
    {
        public SkillRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
