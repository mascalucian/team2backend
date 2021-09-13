using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using team2backend.Controllers;
using team2backend.Data;
using team2backend.Models;
using Xunit;

namespace team2backend.Tests
{
    
    public class SkillControllerTest
    {
        private readonly ApplicationDbContext _context;
        [Fact]
        public void SkillControllerAdd()
        {

        SkillsController controller;

            // Asume

        controller = new SkillsController(_context);

            controller.CreateNewSkill(new Skill
            {
                Name = "test",
                Recomandations = null,
            }
            );

       // Assert.Equal(controller.)
        }

        

    }
}
