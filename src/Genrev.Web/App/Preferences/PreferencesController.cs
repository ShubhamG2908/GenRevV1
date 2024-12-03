using System;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Linq;
using Genrev.Web.App.Account;
using Dymeng;

namespace Genrev.Web.App.Preferences
{

    [Authorize]
    public class PreferencesController : Controller
    {

        private PreferencesService service;

        public PreferencesController()
        {
            this.service = new PreferencesService();
        }

        public ActionResult Edit()
        {
            Models.PreferencesVM model = service.GetPreferences();
            return View("Preferences", model);
        }

        public ActionResult Save(Models.PreferencesVM model)
        {
            service.SavePreferences(model);
            return Json("ok");
        }

        public ActionResult Get(string optionName)
        {
            int userID = AppService.Current.User.UserID;
            var opt = AppService.Current.DataContext.WebUserOptions.SingleOrDefault(x => x.UserID == userID && x.OptionName == optionName);

            if (opt != null)
            {
                return Json(opt.ValueRaw);
            }
            else
            {
                return Json("");
            }

        }

    }
}