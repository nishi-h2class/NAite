using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class FilesController : Controller
    {
        [Route("/files")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
