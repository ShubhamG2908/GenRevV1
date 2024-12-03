using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;
using Genrev.Web.App.Owner.Models;
using System.Collections.ObjectModel;

namespace Genrev.Web.App.Subscription
{

    
    public class OwnerController : ContentAreaController
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

        

        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PageBaseCallback() {

            string tabName = Request.Params["tabName"];

            switch (tabName) {

                case "tabOverview":
                    return getTabOverview();
                case "tabManageExisting":
                    return getTabManageExisting();
                case "tabCreate":
                    return getTabCreate();
                default:
                    throw new ArgumentOutOfRangeException("Tab Name not registered");

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

        #region OverviewTab


        /***********************
        * 
        * OVERVIEW TAB
        * 
        ************************/

        /*--------------------
            GETS
        ---------------------*/
        ActionResult getTabOverview() {
            
            //var model = Models.GeneralVM.MapFromDomain(AppService.GetAccount());
            ViewBag.UIPrefix = "OwnerOverview";
            return PartialView("Overview");
        }

        /*--------------------
            CALLBACKS
        ---------------------*/



        /*--------------------
            POSTS
        ---------------------*/
        [HttpPost]
        public ActionResult CreateSubscription(
            string email, string companyFullName, string companyName,
            string companyCode, string contactFirstName, string contactLastName) {

            var settings = new Domain.Services.Accounts.ProvisionBaseSettings();

            settings.AccountMasterEmail = email;
            settings.MasterCompanyFullName = companyFullName;
            settings.MasterCompanyName = companyName;
            settings.MasterCompanyCode = companyCode;
            settings.MasterLoginFirstName = contactFirstName;
            settings.MasterLoginLastName = contactLastName;
            settings.FiscalYearEndingMonth = Month.December;
            settings.MasterAccountPassword = Domain.Services.Users.Helpers.CreatePassword();
            settings.MasterLoginDisplayName = contactFirstName + ' ' + contactLastName;
            settings.MasterLoginGender = "M";

            if (Domain.Services.Accounts.Provision.ProvisionAccount(settings)) {
                return Content(settings.MasterAccountPassword);
            } else {
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

        #region ExistingTab


        /***********************
        * 
        * EXISTING TAB
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/

        ActionResult getTabManageExisting() {

            var model = new AccountListVM();
            var items = dataContext.Accounts.OrderBy(x => x.Status).ToList();

            foreach (var item in items) {

                model.Items.Add(new AccountListItemVM()
                {
                    Company = item.Companies.Single().FullName,
                    EditLink = "hlOwnerAcctEdit_" + item.ID,
                    Status = item.StatusText,
                    StatusValue = item.Status,
                    Email = item.Email,
                    ID = item.ID
                });

            }
            
            return GetView("Manage", model);
        }

        /*--------------------
            CALLBACKS
        ---------------------*/

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult gvOwnerManageAccountsCallback() {

            var model = new AccountListVM();
            var items = dataContext.Accounts.OrderBy(x => x.Status).ToList();

            foreach (var item in items) {

                model.Items.Add(new AccountListItemVM()
                {
                    Company = item.Companies.Single().FullName,
                    EditLink = "hlOwnerAcctEdit_" + item.ID,
                    Status = item.StatusText,
                    StatusValue = item.Status,
                    Email = item.Email,
                    ID = item.ID
                });

            }

            return GetView("Manage", model.Items);

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

        #region CreateTab


        /***********************
        * 
        * CREATE TAB
        * 
        ************************/


        /*--------------------
            GETS
        ---------------------*/
        ActionResult getTabCreate() {

            var model = new AccountCreateVM();
            ViewBag.UIPrefix = "OwnerSubscriptionCreate";
            return GetView("Create", model);
        }

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
            

        #region ControllerSetup


        /***********************
        * 
        * CONTROLLER SETUP
        * 
        ************************/

        Genrev.Data.GenrevContext dataContext;

        public OwnerController() {
            dataContext = new Genrev.Data.GenrevContext();
        }


        #endregion



    }
}