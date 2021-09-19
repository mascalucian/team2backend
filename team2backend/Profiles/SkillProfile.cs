using AutoMapper;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Profiles
{
    public class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<Skill, GetAllSkillsDto>();

            CreateMap<Skill, GetSkillByIdDto>();

            CreateMap<CreateNewSkillDto, Skill>();
        }
    }
}
