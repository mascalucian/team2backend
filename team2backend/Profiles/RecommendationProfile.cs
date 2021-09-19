using AutoMapper;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Profiles
{
    public class RecommendationProfile : Profile
    {
        public RecommendationProfile()
        {
            CreateMap<GetRecommendationsByUserId, Recomandation>();

            CreateMap<GetRecommendationsBySkillId, Recomandation>();

            CreateMap<Recomandation, CreateRecommendationDto>();
        }
    }
}
