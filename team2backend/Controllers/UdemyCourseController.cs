using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using RestSharp;
using team2backend.Services;

namespace team2backend.Controllers
{
    /// <summary>
    ///   <Udemy Controller. />
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UdemyCourseController : ControllerBase
    {
        private readonly IUdemyCourseService service;

        public UdemyCourseController(IUdemyCourseService service)
        {
            this.service = service;
        }

        [EnableCors("CorsApi")]
        [HttpGet]
        [Route("{searchFor}/{numPage}")]
        public Response Get(string searchFor, int numPage)
        {
            return service.GetResponse(searchFor, numPage);
        }

    }
}
