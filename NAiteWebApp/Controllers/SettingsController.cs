using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("settings/users")]
        public IActionResult UserList()
        {
            return View();
        }

        [Route("settings/users/new")]
        public IActionResult UserAdd()
        {
            return View();
        }

        [Route("settings/users/{id}")]
        public IActionResult UserEdit(string id)
        {
            return View();
        }
    }
}
