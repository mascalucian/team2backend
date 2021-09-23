using Microsoft.AspNetCore.Mvc;

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
