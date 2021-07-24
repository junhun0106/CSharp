using Microsoft.AspNetCore.Mvc;

namespace LatencyView.Controllers
{
    // 리포트는 중요한 자원이므로 IP Check를 해야 한다
    [ServiceFilter(typeof(IpCheckActionFilter))]
    [Route("/")]
    [Controller]
    public class HomeController : Controller
    {
        private readonly StatisticsData _report;

        public HomeController(StatisticsData report)
        {
            _report = report;
        }

        [HttpGet]
        public ActionResult ShowHome()
        {
            return Ok("Hello World!");
        }

        [HttpGet("/report")]
        public ActionResult ShowReport()
        {
            var data = _report.Data;
            return View(viewName: "~/Views/StatisticsView.cshtml", model: new StatsModel { WebApiInfos = data });
        }
    }
}
