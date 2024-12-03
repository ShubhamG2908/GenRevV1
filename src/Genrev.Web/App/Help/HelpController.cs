using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;

namespace Genrev.Web.App.Help
{
    public class HelpController : Controller
    {


        public ActionResult GetTopic(int topicID) {
            return PartialView(getViewNameFromTopicID(topicID));
        }


        string getViewNameFromTopicID(int topicID) {
            return "Topic" + topicID;
        }
            

    }
}