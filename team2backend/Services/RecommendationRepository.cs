using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Data;
using team2backend.Data.EfCore;
using team2backend.Models;

namespace team2backend.Services
{
    public class RecommendationRepository : EfCoreRepository<Recomandation, ApplicationDbContext>
    {
        private ApplicationDbContext context;
        public RecommendationRepository(ApplicationDbContext context) : base(context)
        {

            this.context = context;
        }

        public List<Recomandation> GetRecomandationsForSkill(int id)
        {
            var recommendations = GetAll();
            return recommendations.FindAll(_ => _.SkillId == id);
        }
    }
}
