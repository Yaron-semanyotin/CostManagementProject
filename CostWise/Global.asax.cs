using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace CostWise
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
        void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            string requestPath = Request.AppRelativeCurrentExecutionFilePath;
            if (requestPath.StartsWith("~/api/", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.ReadOnly);
            }
        }
    }
}