using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace team2backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class HealthController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {

            return Json(new { Version = "Version 1.0" });
        }

    }
}
