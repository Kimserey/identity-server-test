using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerTest.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        [HttpGet, Authorize]
        public IActionResult Get()
        {
            return new JsonResult(
                new int[] { 1, 2, 3, 4, 5 }
            );
        }

        [HttpGet, Authorize("api.call"), Route("makecall")]
        public IActionResult MakeCall()
        {
            return Ok();
        }
    }
}
