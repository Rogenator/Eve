using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eve.Web.Controllers
{
    public class CompanyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string fileName = string.Format("{0}{1}", Guid.NewGuid().ToString("N").ToUpper(), System.IO.Path.GetExtension(file.FileName));
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/Images/Companies"), fileName);
                file.SaveAs(path);
                return Json(new { Message = fileName, Status = true });
            }
            return Json(new { Message = "No File", Status = false });
        }
        public int GetCount(string like)
        {
            return new Business.Services.CompanyService().GetCount(like);
        }
        public JsonResult Get(Business.Util.GridOptions options, string like)
        {
            return Json(new Business.Services.CompanyService().Get(options, like));
        }
        public int Save(Business.Objects.Company company)
        {
            return new Business.Services.CompanyService().Save(company);
        }
    }
}
