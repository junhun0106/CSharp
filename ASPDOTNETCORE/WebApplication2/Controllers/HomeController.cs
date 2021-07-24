using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("/")]
    [Controller]
    [UsedImplicitly]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult ShowHone()
        {
            return Ok("Hello World!");
        }
    }
}
