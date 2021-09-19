using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<MessageHub> hub;
        [Fact]
        public /*async*/ void SkillControllerAdd()
        {

            SkillsController controller;

            // Asume

            //controller = new SkillsController(_context, hub);

            /*await*/
            //controller.CreateNewSkill(new Skill
            {
                //Name = "test",
                //Recomandations = null,
            }
  //);
        }



    }
}
