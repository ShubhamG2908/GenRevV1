using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;

namespace Genrev.Web.App.Subscription
{

    [Authorize(Roles = "sysadmin")]
    public class SubscriptionController : ContentAreaController
    {


        #region CorePage

        /***********************
        * 
        * CORE PAGE
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/
        [HttpGet]
        public ActionResult Index() {
            return GetView("PageBase");
        }

        


        ActionResult getTabSubscription() {
            return PartialView("Subscription");
        }

        ActionResult getTabActivity() {
            return PartialView("Activity");
        }

        ActionResult getTabIntegrations() {

            var model = _service.GetApiVM();
                
            return PartialView("Integrations", model);
        }
        

        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PageBaseCallback() {

            string tabName = Request.Params["tabName"];

            switch (tabName) {

                case "tabGeneral":
                    return getTabGeneral();
                case "tabSubscription":
                    return getTabSubscription();
                case "tabActivity":
                    return getTabActivity();
                case "tabIntegrations":
                    return getTabIntegrations();                    
                default:
                    throw new ArgumentOutOfRangeException();

            }
            
        }
        


        /*--------------------
            POSTS
        ---------------------*/



        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/




        #endregion

        #region GeneralTab


        /***********************
        * 
        * GENERAL TAB
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/
        ActionResult getTabGeneral() {
            
            var model = Models.GeneralVM.MapFromDomain(AppService.Current.Account);
            ViewBag.UIPrefix = "SubscriptionGeneral";
            return PartialView("General", model);
        }

        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/
        [HttpPost]
        public ActionResult General(
            string companyFullName, string companyName, 
            string companyCode, string contactFirstName, string contactLastName) {
            
            var account = AppService.Current.Account;
            
            account.PrimaryCompany.FullName = companyFullName;
            account.PrimaryCompany.Name = companyName;
            account.PrimaryCompany.Code = companyCode;
            account.SysAdminPerson.FirstName = contactFirstName;
            account.SysAdminPerson.LastName = contactLastName;

            try {
                dataContext.SaveChanges();
                return Content("ok");
            } catch (Exception e) {
                AppService.Current.LogException(e);
                return Content("err");
            }
        }


        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/


        #endregion

        #region SubscriptionTab


        /***********************
        * 
        * SUBSCRIPTION TAB
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/


        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/



        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/



        #endregion

        #region ActivityTab


        /***********************
        * 
        * ACTIVITY TAB
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/


        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/



        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/




        #endregion

        #region IntegrationsTab


        /***********************
        * 
        * INTEGRATIONS TAB
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/


        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/

        [HttpPost]
        public ActionResult SaveApiSettings(bool apiEnabled, string apiKey, string apiPassword) {

            _service.SaveApiSettings(apiEnabled, apiKey, apiPassword);

            return Content("ok");
        }

        [HttpPost]
        public ActionResult GetNewApiKey() {
            try {
                var key = _service.GetNewApiKey();
                return Content(key);
            } catch {
                return Content("error");
            }
            
            
        }

        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/



        #endregion

        #region ControllerSetup


        /***********************
        * 
        * CONTROLLER SETUP
        * 
        ************************/

        Genrev.Data.GenrevContext dataContext;
        SubscriptionService _service;

        public SubscriptionController() {
            dataContext = AppService.Current.DataContext;
            _service = new SubscriptionService();
        }


        #endregion



    }
}