using Microsoft.AspNetCore.Mvc;

namespace NAiteWebApp.Controllers
{
    public class ItemDataImportController : Controller
    {
        [Route("itemDataImports")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("itemDataImports/new")]
        public IActionResult Add()
        {
            return View();
        }

        [Route("itemDataImports/{id}")]
        public IActionResult Edit(string id)
        {
            return View();
        }
    }
}
