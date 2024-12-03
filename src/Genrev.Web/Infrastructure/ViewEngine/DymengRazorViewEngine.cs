using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Genrev.Web.Infrastructure.ViewEngine
{

    public class DymengRazorViewEngine : RazorViewEngine
    {

        //http://weblogs.asp.net/imranbaloch/view-engine-with-dynamic-view-location

        private IViewPageActivator activator;


        public DymengRazorViewEngine() : base() {
            initPaths();
        }

        public DymengRazorViewEngine(IViewPageActivator activator) {
            this.activator = activator;
            initPaths();
        }
        

        void initPaths() {
            AreaViewLocationFormats = new[]
            {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            AreaMasterLocationFormats = new[]
            {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            AreaPartialViewLocationFormats = new[]
            {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            ViewLocationFormats = new[]
            {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App//Shared/Views/{0}.cshtml"
            };

            MasterLocationFormats = new[]
            {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App/Shared/Views/{0}.cshtml"
            };

            PartialViewLocationFormats = new[]
            {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App/Shared/Views/{0}.cshtml"
            };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath) {
            return base.CreatePartialView(controllerContext, partialPath);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath) {
            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath) {
            return base.FileExists(controllerContext, virtualPath);
        }


        
    }
}