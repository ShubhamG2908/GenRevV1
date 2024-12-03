using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace Genrev.Web.App.Sample
{
    public class SampleController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {

        public ActionResult Dashboard() {
            return GetView("Dashboard");
        }

        public ActionResult Search() {
            return GetView("Search");
        }

    }
}