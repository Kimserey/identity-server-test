using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerTest.WebApi.Controllers
{
    [Route("api/[controller]")]
	[Authorize]
    public class DataController : Controller
    {
        [HttpGet]
		public IActionResult Get()
		{
			return new JsonResult(
				new int[] { 1, 2, 3, 4, 5 }
			);
		}
    }
}
