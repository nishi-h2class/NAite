using Microsoft.AspNetCore.Mvc;
using NAiteWebApp.Models;
using System.Diagnostics;

namespace NAiteWebApp.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        [Route("/home")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
