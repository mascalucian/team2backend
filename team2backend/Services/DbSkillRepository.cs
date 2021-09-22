using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json.Linq;
using team2backend.Controllers;
using team2backend.Data;
using team2backend.Dtos;
using team2backend.Interfaces;
using team2backend.Models;

namespace team2backend.Services
{
    public class DbSkillRepository : ISkillsRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IUdemyCourseService udemyCourseService;

        public DbSkillRepository(ApplicationDbContext context, IMapper mapper, IUdemyCourseService udemyCourseService)
        {
            _context = context;
            this.mapper = mapper;
            this.udemyCourseService = udemyCourseService;
        }

        public Skill CreateNewSkill(Skill skill)
        {
            var checkSkill = _context.Skills
                   .FirstOrDefault(m => m.Name == skill.Name);
            if (checkSkill != null) return null;
            var hasResults = udemyCourseService.HasResults(skill.Name);
            if (!hasResults) return null;
            _context.Add(skill);
            _context.SaveChanges();
            return skill;
        }

        public void Delete(int id)
        {
            var skill = _context.Skills.Find(id);
            _context.Skills.Remove(skill);
            _context.Recomandations.RemoveRange(_context.Recomandations.Where(_ => _.SkillId == id));
            _context.Skills.RemoveRange(_context.Skills.Where(_ => _.ParentId == id));
            _context.SaveChanges();
        }

        public Skill Edit(int id, Skill updatedSkill)
        {
            var skillToUpdate = _context.Skills.Find(id);

            if (skillToUpdate != null)
            {
                skillToUpdate.Name = updatedSkill.Name;
                var recommendations = _context.Recomandations.Where(_ => _.SkillId == id).ToList();
                recommendations.ForEach(_ => _.SkillName = skillToUpdate.Name);
                _context.Recomandations.UpdateRange(recommendations);
                _context.SaveChanges();
                return skillToUpdate;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<GetAllSkillsDto> GetAllSkills()
        {
            var skills = _context.Skills.ToList();
            var getAllSkillsDto = mapper.Map<IEnumerable<GetAllSkillsDto>>(skills);
            return getAllSkillsDto;
        }

        public Skill GetSkillById(int id)
        {
            var skill = _context.Skills
                    .FirstOrDefault(m => m.Id == id);
            return skill;
        }
    }
}
