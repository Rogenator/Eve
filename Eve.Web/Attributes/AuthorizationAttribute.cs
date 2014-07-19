using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Eve.Web.Attributes
{
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        protected System.Web.SessionState.HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Session["CurrentUser"] == null)
                filterContext.Result = new RedirectResult("~/LogIn");
            else
            {
                //Validate
            }
        }
    }
}