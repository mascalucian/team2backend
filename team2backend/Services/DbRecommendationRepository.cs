using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using team2backend.Data;
using team2backend.Dtos;
using team2backend.Interfaces;
using team2backend.Models;

namespace team2backend.Services
{
    public class DbRecommendationRepository : IRecommendationsRepository
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public DbRecommendationRepository(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public object CreateRecomandation(Recomandation recomandation)
        {
            var skill = context.Skills
                    .FirstOrDefault(_ => _.Id == recomandation.SkillId);
            recomandation.SkillName = skill.Name;
            context.Recomandations.Add(recomandation);
            context.SaveChanges();
            var response = new { recomandation, skill };
            return response;
        }

        public void DeleteRecomandation(int id)
        {
            var recomandation = context.Recomandations.Find(id);
            context.Recomandations.Remove(recomandation);
            context.SaveChanges();
        }

        public void Edit(int id, EditRecommendationDto recomandationUpdatedDto)
        {
            var recomandationToUpdate = context.Recomandations.Find(id);

            var recommendation = mapper.Map<Recomandation>(recomandationUpdatedDto);
            recomandationToUpdate.Rating = recommendation.Rating;
            recomandationToUpdate.Feedback = recommendation.Feedback;
            context.SaveChanges();
        }

        public IEnumerable<Recomandation> GetRecomandationsBySkillId(int skillId)
        {
            var recomandations = context.Recomandations.Where(sId => sId.SkillId == skillId)
                .Distinct().ToList();
            return recomandations;
        }

        public IEnumerable<Recomandation> GetRecomandationsForUser(string userId)
        {
            var recomandations = context.Recomandations.Where(recommendationId => recommendationId.UserId == userId)
                 .Distinct().ToList();
            return recomandations;
        }
    }
}
