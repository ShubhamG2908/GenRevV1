using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;
using Genrev.Web.App.Personnel.Models;

namespace Genrev.Web.App.Personnel
{
    [Authorize(Roles = "sysadmin")]
    public class PersonnelController : ContentAreaController
    {




        

        /*--------------------
            GETS
        ---------------------*/

        [HttpGet]
        public ActionResult Index() {
            return Overview();
        }

        [HttpGet]
        public ActionResult Overview() {

            ViewBag.UIPrefix = "personnelmgmt";

            var account = AppService.Current.Account;
            var model = new OverviewVM();

            model.Personnel = service.GetOverviewListItems(account);

            return GetView("Overview", model);
        }

        [HttpGet]
        public ActionResult AddNewPopup() {
            var model = new AddNewVM();
            return PartialView("AddNewPopup", model);
        }

        [HttpGet]
        public ActionResult LoginInfo(int id) {

            var user = dataContext.Users.Where(x => x.PersonnelID == id).FirstOrDefault();

            if (user == null) {
                var model = new CreateLoginInfoVM();
                return PartialView("CreateLoginInfo", model);
            } else {
                var model = new EditLoginInfoVM();
                model.EditLoginInfoDisplayName = user.DisplayName;
                model.EditLoginInfoEmail = user.Email;
                return PartialView("EditLoginInfo", model);
            }
        }

        [HttpGet]
        public ActionResult ReportsToGrid(int personnelID) {

            ViewBag.UIPrefix = "personnelmgmt";
            
            var account = AppService.Current.Account;

            var model = service.GetReportsToListItems(account, personnelID);

            return GetView("ReportsToGrid", model);

        }


        [HttpGet]
        public ActionResult AvailabilityPopup(int personnelID) {

            var model = new AvailabilityPopupVM();
            model.GridVM = new AvailabilityPopupGridVM();
            model.GridVM.Items = service.GetAvailabilityListItems(personnelID);

            model.PersonName = AppService.Current.DataContext.Personnel.Find(personnelID)?.CommonName;

            return PartialView("AvailabilityPopup", model);

        }
        
        [HttpPost]
        public ActionResult ToggleAdmin(int personnelID)
        {
            if(AppService.Current.Person.PersonID == personnelID)
            {
                return Json("You cannot revoke your own administrative access.");
            }

            service.ToggleAdmin(personnelID);
            return Json("ok");
        }

        /*--------------------
            CALLBACKS
        ---------------------*/
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PersonnelOverviewGridCallback() {

            ViewBag.UIPrefix = "personnelmgmt";
            
            return GetView("OverviewGrid", service.GetOverviewListItems(AppService.Current.Account));

        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ReportsToGridCallback() {

            ViewBag.UIPrefix = "personnelmgmt";

            int personnelID = int.Parse(Request.Params["personnelID"]);
            var account = AppService.Current.Account;

            var model = service.GetReportsToListItems(account, personnelID);
            
            return GetView("ReportsToGrid", model);

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AvailabilityPopupGridCallback() {

            var personnelID = int.Parse(Request.Params["personnelID"]);
            var model = new AvailabilityPopupGridVM();

            model.Items = service.GetAvailabilityListItems(personnelID);

            return PartialView("AvailabilityPopupGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AvailabilityPopupBatchEdit(DevExpress.Web.Mvc.MVCxGridBatchUpdateValues<AvailabilityListItem, int> updateValues) {

            var personnelID = int.Parse(Request.Params["personnelID"]);
            
            try {
                if (updateValues.DeleteKeys.Count > 0) {
                    service.DeleteAvailabilityItems(updateValues.DeleteKeys);
                }

                if (updateValues.Insert.Count > 0) {
                    service.InsertAvailabilityItems(updateValues.Insert, personnelID);
                }

                if (updateValues.Update.Count > 0) {
                    service.UpdateAvailabilityItems(updateValues.Update);
                }
            } catch {
                return Content("Unable to make the requested changes.  Please ensure the changes doesn't cause any years to be duplicated.");
            }
            
            
            return TransferToAction("AvailabilityPopupGridCallback");

        }



        /*--------------------
            POSTS
        ---------------------*/

        [HttpPost]
        public ActionResult ToggleReportsTo(int sourceID, int targetID) {
            
            try {
                service.ToggleReportsTo(sourceID, targetID);
                return Content("ok");
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Content("err: internal error");
            }

        }

        [HttpPost]
        public ActionResult Remove(int id) {

            if (AppService.Current.Person.PersonID == id) {
                return Content("err: You cannot remove yourself from the system.");
            }

            var person = dataContext.Personnel.Find(id);
            var user = dataContext.Users.Where(x => x.PersonnelID == person.ID).FirstOrDefault();

            try {
                if (user != null) {
                    dataContext.Users.Remove(user);
                }
                dataContext.Personnel.Remove(person);
                dataContext.SaveChanges();

                return Content("ok");

            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Content("err: internal error");
            }
        }

        [HttpPost]
        public ActionResult AddNew(string firstName, string lastName, bool isAdmin) {

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) {
                return Content("err: null");
            }

            var account = AppService.Current.Account;
            var person = account.PrimaryCompany.Personnel.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefault();
            if (person != null) {
                return Content("err: name already exists");
            }

            var newPerson = new Domain.Companies.Person();

            newPerson.FirstName = firstName;
            newPerson.LastName = lastName;
            newPerson.CompanyID = account.PrimaryCompany.ID;

            if (isAdmin)
            {
                newPerson.AddRole(dataContext.Roles.First(x => x.IsSysAdministrator));
            }

            newPerson.AddRole(dataContext.Roles.First(x => x.IsSysSalesPro));

            
            try {
                dataContext.Personnel.Add(newPerson);
                dataContext.SaveChanges();
                return Content("ok");
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Content("err: internal error");
            }
            
        }


        [HttpPost]
        public ActionResult RemoveLogin(int personnelID) {

            if (AppService.Current.Person.PersonID == personnelID) {
                return Content("err: You cannot remove your own login information.");
            }

            var user = dataContext.Users.Where(x => x.PersonnelID == personnelID).FirstOrDefault();
            
            if (user == null) {
                return Content("ok");
            }

            try {
                dataContext.Users.Remove(user);
                dataContext.SaveChanges();
                return Content("ok");
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Content("err: internal error");
            }
            
            
        }

        [HttpPost]
        public ActionResult CreateLogin(int personnelID, string email, string displayName, string password) {

            var account = AppService.Current.Account;

            if (!Domain.Services.Users.Helpers.IsLegalPassword(password)) {
                return Content("err: The password is not valid.  Please ensure it meets the minimum password length and try again.");
            }

            var dupeEmails = dataContext.Users.Where(x => x.Email == email).FirstOrDefault();
            if (dupeEmails != null) {
                return Content("err: A user with this email already exists.  Unable to create.");
            }

            var user = new Domain.Users.User();
            user.AccountID = account.ID;
            user.PersonnelID = personnelID;
            user.DisplayName = displayName;
            user.Email = email;
            user.MembershipDetail = new Domain.Users.WebMembership();
            user.MembershipDetail.Password = Domain.Services.Users.Helpers.HashPassword(password);

            try {
                dataContext.Users.Add(user);
                dataContext.SaveChanges();
                return Content("ok");
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Content("err: internal error");
            }



        }

        [HttpPost]
        public ActionResult UpdateLogin(int personnelID, string email, string displayName, string password) {

            var account = AppService.Current.Account;

            var user = dataContext.Users.Where(x => x.PersonnelID == personnelID).FirstOrDefault();

            if (user == null) {
                throw new InvalidOperationException("User not found, user to personnel mismatch");
            }

            user.DisplayName = displayName;
            user.Email = email;

            if (!string.IsNullOrEmpty(password)) {
                if (!Domain.Services.Users.Helpers.IsLegalPassword(password)) {
                    return Content("err: The password is not valid.  Please ensure it meets the minimum password length and try again.");
                }
                var hashedPassword = Domain.Services.Users.Helpers.HashPassword(password);
                user.MembershipDetail.Password = hashedPassword;
            }

            dataContext.SaveChanges();

            return Content("ok");
        }


        /*--------------------
            DELETES
        ---------------------*/



        /*--------------------
            HELPERS
        ---------------------*/





        /***********************
        * 
        * CONTROLLER SETUP
        * 
        ************************/

        Genrev.Data.GenrevContext dataContext;
        PersonnelDataService service;

        public PersonnelController() {
            dataContext = new Genrev.Data.GenrevContext();
            service = new PersonnelDataService(dataContext);

        }


    }
}