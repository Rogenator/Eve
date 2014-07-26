using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eve.Web.Controllers
{
    public class MetaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public int GetCount(string like)
        {
            return new Business.Services.MetaService().GetCount(like);
        }
        public JsonResult Get(Business.Util.GridOptions options, string like)
        {
            return Json(new Business.Services.MetaService().Get(options, like));
        }
        public JsonResult GetAll()
        {
            return Json(new Business.Services.MetaService().GetAll());
        }
        public int Save(Business.Objects.Meta meta)
        {
            return new Business.Services.MetaService().Save(meta);
        }
        public int Delete(int metaId)
        {
            return new Business.Services.MetaService().Delete(metaId);
        }
    }
}
