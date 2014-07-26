using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eve.Web.Controllers
{
    public class ProductController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public int GetCount(Business.Services.ProductFiltering filter)
        {
            //Get from Session
            filter.CompanyId = 1;
            return new Business.Services.ProductService().GetCount(filter);
        }

        public JsonResult Get(Business.Util.GridOptions options, Business.Services.ProductFiltering filter)
        {
            //Get from Session
            filter.CompanyId = 1;
            return Json(new Business.Services.ProductService().Get(options, filter));
        }

        public JsonResult GetImages(int productId)
        {
            return Json(new Business.Services.ProductService().GetImages(productId));
        }

        public JsonResult GetMetas(int productId)
        {
            return Json(new Business.Services.ProductService().GetMetas(productId));
        }

        public JsonResult GetInventory(int productId)
        {
            return Json(new Business.Services.ProductService().GetInventory(productId));
        }

        public JsonResult Save(Business.Objects.Product product, List<Business.Objects.ProductImage> images, List<Business.Objects.ProductMeta> metas)
        {
            //Get from Session
            product.CompanyId = 1;
            return Json(new Business.Services.ProductService().Save(product, images, metas));
        }

        public int SaveInventory(List<Business.Objects.ProductMeta> metaList)
        {
            return new Business.Services.ProductService().SaveInventory(metaList);
        }

        public JsonResult UploadImage(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string guid = Guid.NewGuid().ToString("N").ToUpper();
                string fileName = string.Format("{0}{1}", guid, System.IO.Path.GetExtension(file.FileName));
                string thumbName = string.Format("thumb_{0}{1}", guid, System.IO.Path.GetExtension(file.FileName));
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/Images/Products"), fileName);
                string thumbPath = System.IO.Path.Combine(Server.MapPath("~/Content/Images/Products"), thumbName);
                file.SaveAs(path);
                this.GenerateThumb(path).Save(thumbPath);
                return Json(new { Message = fileName, Status = true });
            }
            return Json(new { Message = "No File", Status = false });
        }
    }
}