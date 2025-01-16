using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class AnalysisController : Controller
    {
        [Route("/analysis/{id}")]
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}
