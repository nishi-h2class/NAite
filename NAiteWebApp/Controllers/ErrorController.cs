using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{id}")]
        public ActionResult Index(string id)
        {
            var msg = id + "エラーページです。";

            ViewData["Title"] = id;
            ViewData["Description"] = msg;
            return View();
        }
    }
}
