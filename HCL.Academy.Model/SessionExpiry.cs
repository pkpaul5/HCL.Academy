using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HCL.Academy.Model
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // check  sessions here


            if (HttpContext.Current.Session["CurrentUser"] == null)
            {
                FormsAuthentication.SignOut();

                filterContext.Result = new RedirectResult("~/Authorize/Logout");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}