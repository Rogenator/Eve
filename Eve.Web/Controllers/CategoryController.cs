using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eve.Web.Controllers
{
    public class CategoryController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategories()
        {
            return Json(new Business.Services.CategoryService().GetCategories(string.Empty));
        }
        public JsonResult SaveCategory(Business.Objects.Category cat)
        {
            return Json(new Business.Services.CategoryService().SaveCategory(cat));
        }
        public JsonResult GetCategoriesList()
        {
            return Json(new Business.Services.CategoryService().GetCategoriesList());
        }
    }
}
