using Serilog;
using System;
using System.IO;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Genrev.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ExceptionHandlingConfig.RegisterExceptionHandler();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AuthConfig.RegisterAuth();
            ViewEngineConfig.RegisterViewEngine(new Infrastructure.ViewEngine.DymengRazorViewEngine());
            
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

            DevExpress.Web.ASPxWebControl.CallbackError += Application_Error;
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log-.txt");  
            string logFolderPath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application Started");
        }

        protected void Application_Error(object sender, EventArgs e) 
        {
            Exception exception = System.Web.HttpContext.Current.Server.GetLastError();
            //TODO: Handle Exception
        }
        protected void Application_End(object sender, EventArgs e)
        {
            Log.Information("Application Ended");
            Log.CloseAndFlush();
        }
    }
}