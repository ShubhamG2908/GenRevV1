using Serilog;

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Genrev.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly string DefaultCultureCode = "en-US";
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

            SetGlobalCulture(DefaultCultureCode);
        }
        
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userCountryCode = GetLoggedInUserCountryCode();
                if (!string.IsNullOrEmpty(userCountryCode))
                {
                    HttpContext.Current.Items["TempGenRevCountryCode"] = userCountryCode;                    
                }
            }
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = HttpContext.Current.Server.GetLastError();
        }
        protected void Application_End(object sender, EventArgs e)
        {
            Log.Information("Application Ended");
            Log.CloseAndFlush();
        }
        private void SetGlobalCulture(string countryCode)
        {
            try
            {
                var newCulture = new CultureInfo(countryCode);
                var regionInfo = new RegionInfo(newCulture.Name);
                newCulture.NumberFormat.CurrencySymbol = regionInfo.CurrencySymbol;

                Thread.CurrentThread.CurrentCulture = newCulture;
                Thread.CurrentThread.CurrentUICulture = newCulture;
            }
            catch (CultureNotFoundException)
            {
                var fallback = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = fallback;
                Thread.CurrentThread.CurrentUICulture = fallback;
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (HttpContext.Current?.Session != null)
            {
                var userCountryCode = HttpContext.Current.Items["TempGenRevCountryCode"] as string;
                if (!string.IsNullOrEmpty(userCountryCode))
                {
                    Session["GenRevCountryCode"] = CurrencyHelper.SetGlobalCulture(userCountryCode);
                    SetGlobalCulture(userCountryCode);
                }
            }
        }

        private string GetLoggedInUserCountryCode()
        {
            try
            {
                return AppService.Current.Account.PrimaryCompany.CountryCode;
            }
            catch
            {
                return null;
            }
        }
    }
}