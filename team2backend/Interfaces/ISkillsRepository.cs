using System.Collections.Generic;
using team2backend.Dtos;
using team2backend.Models;

namespace team2backend.Interfaces
{
    public interface ISkillsRepository
    {
        Skill CreateNewSkill(Skill skill);

        void Delete(int id);

        Skill Edit(int id, Skill updatedSkill);

        IEnumerable<GetAllSkillsDto> GetAllSkills();

        GetSkillByIdDto GetSkillById(int id);
    }
}