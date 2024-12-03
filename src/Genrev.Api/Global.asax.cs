using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Genrev.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            ViewEngineConfig.RegisterViewEngine(new Infrastructure.ViewEngine.DymengRazorViewEngine());

        }
        
    }
}
