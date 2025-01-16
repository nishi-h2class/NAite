using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
