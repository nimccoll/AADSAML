using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ASPNetB2CSAML
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest()
        {
            if (this.Request.Cookies["AzureADAuth"] != null)
            {
                string userName = this.Request.Cookies["AzureADAuth"]["UserName"];
                GenericIdentity identity = new GenericIdentity(userName);
                GenericPrincipal principal = new GenericPrincipal(identity, null);
                this.Context.User = principal;
                Thread.CurrentPrincipal = principal;
            }
        }
    }
}
