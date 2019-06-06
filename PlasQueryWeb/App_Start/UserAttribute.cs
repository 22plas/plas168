using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.App_Start
{
    public class UserAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies["AccountData"] == null)
            {
                filterContext.HttpContext.Response.Redirect("/MemberCenter/Login");
            }
        }
    }
}