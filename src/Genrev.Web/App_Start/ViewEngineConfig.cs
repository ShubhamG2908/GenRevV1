using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Genrev.Web
{
    public static class ViewEngineConfig
    {
        public static void RegisterViewEngine(RazorViewEngine engine) {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine);
        }

    }
}