using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;
using Genrev.Web.App.Customers;

namespace Genrev.Web.App.Customers
{

    [Authorize(Roles = "sysadmin")]
    public class CustomersController : ContentAreaController
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
        #region Gets


        [HttpGet]
        public ActionResult GetMappingCustomerList(int personnelID) {

            var model = service.GetCustomerMappingsListItems(personnelID);

            return PartialView("MappingsCustomersGrid", model);

        }
        
        [HttpGet]
        public ActionResult Index() {
            return GetView("PageBase");
        }
        
        [HttpGet]
        public ActionResult AddNewCustomerPopup() {

            var model = new Models.AddCustomerVM();

            model.CustomerTypesList = CommonListItems.DataService.GetCustomerTypeCommonList();
            model.IndustriesList = CommonListItems.DataService.GetIndustryCommonList();
            model.AccountTypesList = CommonListItems.DataService.GetAccountTypeCommonList();

            return PartialView("AddNewCustomerPopup", model);
        }

        [HttpGet]
        public ActionResult EditCustomerTypePopup(int id) {

            var model = new Models.EditCustomerTypeVM();
            var type = dataContext.CustomerTypes.Find(id);

            model.ID = id;
            model.Name = type.Name;

            return PartialView("EditTypePopup", model);
        }

        [HttpGet]
        public ActionResult EditIndustryPopup(int id) {

            var model = new Models.EditIndustryVM();
            var ind = dataContext.Industries.Find(id);

            model.ID = ind.ID;
            model.Name = ind.Name;

            return PartialView("EditIndustryPopup", model);
        }

        [HttpGet]
        public ActionResult EditAccountTypePopup(int id) {

            var model = new Models.EditAccountTypeVM();
            var acctType = dataContext.AccountTypes.Find(id);

            model.ID = acctType.ID;
            model.Name = acctType.Name;
            model.CallsPerMonthGoal = acctType.CallsPerMonthGoal;

            return PartialView("EditAccountTypePopup", model);
        }

        [HttpGet]
        public ActionResult EditCustomerPopup(int id) {

            var model = new Models.EditCustomerVM();
            
            model.CustomerTypesList = CommonListItems.DataService.GetCustomerTypeCommonList();
            model.IndustriesList = CommonListItems.DataService.GetIndustryCommonList();
            model.AccountTypesList = CommonListItems.DataService.GetAccountTypeCommonList();

            var customer = dataContext.Customers.Find(id);

            model.Address1 = customer.Address1;
            model.Address2 = customer.Address2;
            model.City = customer.City;
            model.Country = customer.Country;
            model.CustomerTypeCLI = 
                customer.TypeID.HasValue 
                ? model.CustomerTypesList.Where(x => x.ID == customer.TypeID.Value).SingleOrDefault() 
                : null;
            model.ID = customer.ID;
            model.IndustryCLI =
                customer.IndustryID.HasValue
                ? model.IndustriesList.Where(x => x.ID == customer.IndustryID.Value).SingleOrDefault()
                : null;
            model.AccountTypeCLI =
                customer.AccountTypeID.HasValue
                ? model.AccountTypesList.Where(x => x.ID == customer.AccountTypeID.Value).SingleOrDefault()
                : null;
            model.Name = customer.Name;
            model.Phone = customer.Phone;
            model.PostalCode = customer.PostalCode;
            model.State = customer.State;
            
            return PartialView("EditCustomerPopup", model);
        }


        ActionResult getTabClassifications() {

            var model = new Models.ClassificationVM();
            int accountID = AppService.Current.Account.PrimaryCompany.ID;

            var types = dataContext.CustomerTypes.Where(x => x.CompanyID == accountID).ToList();
            var industries = dataContext.Industries.Where(x => x.CompanyID == accountID).ToList();
            var accountTypes = dataContext.AccountTypes.Where(x => x.CompanyID == accountID).ToList();

            model.Industries = new List<Models.IndustryListItemVM>();
            model.Types = new List<Models.TypesListItemVM>();
            model.AccountTypes = new List<Models.AccountTypeListItemVM>();

            foreach( var industry in industries) {
                model.Industries.Add(new Models.IndustryListItemVM()
                {
                    ID = industry.ID,
                    Name = industry.Name
                });
            }

            foreach (var type in types) {
                model.Types.Add(new Models.TypesListItemVM()
                {
                    ID = type.ID,
                    Name = type.Name
                });
            }

            foreach (var accountType in accountTypes) {
                model.AccountTypes.Add(new Models.AccountTypeListItemVM()
                {
                    ID = accountType.ID,
                    Name = accountType.Name,
                    CallsPerMonthGoal = accountType.CallsPerMonthGoal
                });
            }

            return PartialView("Classifications", model);
        }

        ActionResult getTabCustomers() {

            var model = new Models.CustomersVM();
            model.CustomerListItems = service.GetCustomerListItems();
                        
            return PartialView("Customers", model);
        }

        ActionResult getTabMappings() {

            var model = new Models.MappingVM();
            var account = AppService.Current.Account;
            var domainPersonnel = account.PrimaryCompany.Personnel;
                        
            model.Personnel = new List<Models.MappingPersonnelListItemVM>();

            foreach (var dp in domainPersonnel) {
                var p = new Models.MappingPersonnelListItemVM();
                p.ID = dp.ID;
                p.FirstName = dp.FirstName;
                p.LastName = dp.LastName;
                model.Personnel.Add(p);
            }
            
            return PartialView("Mappings", model);
        }



        #endregion

        /*--------------------
            CALLBACKS
        ---------------------*/
        #region Callbacks

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CustomerMappingsCustomerGridCallback() {

            int personnelID = int.Parse(Request.Params["personnelID"]);
            var model = service.GetCustomerMappingsListItems(personnelID);
            
            return PartialView("MappingsCustomersGrid", model);
            
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CustomerMappingsPersonnelGridCallback() {

            var model = new Models.MappingVM();
            var account = AppService.Current.Account;
            var domainPersonnel = account.PrimaryCompany.Personnel;

            model.Personnel = new List<Models.MappingPersonnelListItemVM>();

            foreach (var dp in domainPersonnel) {
                var p = new Models.MappingPersonnelListItemVM();
                p.ID = dp.ID;
                p.FirstName = dp.FirstName;
                p.LastName = dp.LastName;
                model.Personnel.Add(p);
            }

            return PartialView("MappingsPersonnelGrid", model.Personnel);

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CustomersGridCallback() {

            var model = new Models.CustomersVM();
            model.CustomerListItems = service.GetCustomerListItems();                

            return PartialView("CustomersGrid", model);

        }
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CustomerIndustriesCallback() {

            var model = new List<Models.IndustryListItemVM>();

            var account = AppService.Current.Account;
            var domainIndustries = dataContext.Industries.Where(x => x.CompanyID == account.PrimaryCompany.ID).ToList();

            foreach (var ind in domainIndustries) {
                model.Add(new Models.IndustryListItemVM()
                {
                    ID = ind.ID,
                    Name = ind.Name
                });
            }

            return PartialView("IndustriesGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CustomerTypesCallback() {

            var model = new List<Models.TypesListItemVM>();

            var account = AppService.Current.Account;
            var domainTypes = dataContext.CustomerTypes.Where(x => x.CompanyID == account.PrimaryCompany.ID).ToList();

            foreach (var t in domainTypes) {
                model.Add(new Models.TypesListItemVM()
                {
                    ID = t.ID,
                    Name = t.Name
                });
            }

            return PartialView("TypesGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AccountTypesCallback() {

            var model = new List<Models.AccountTypeListItemVM>();

            var account = AppService.Current.Account;
            var domainTypes = dataContext.AccountTypes.Where(x => x.CompanyID == account.PrimaryCompany.ID).ToList();

            foreach (var t in domainTypes) {
                model.Add(new Models.AccountTypeListItemVM()
                {
                    ID = t.ID,
                    Name = t.Name, 
                    CallsPerMonthGoal = t.CallsPerMonthGoal
                });
            }

            return PartialView("AccountTypesGrid", model);

        }
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PageBaseCallback() {

            string tabName = Request.Params["tabName"];

            switch (tabName) {

                case "tabCustomers":
                    return getTabCustomers();
                case "tabClassifications":
                    return getTabClassifications();
                case "tabMappings":
                    return getTabMappings();
                default:
                    throw new ArgumentOutOfRangeException();

            }

        }


        #endregion



        /*--------------------
            POSTS
        ---------------------*/
        #region Posts


        [HttpPost]
        public ActionResult ToggleMapping(int personnelID, int customerID) {

            service.ToggleCustomerMapping(personnelID, customerID);

            return Content("ok");
        }
        
        [HttpPost]
        public ActionResult AddCustomer(
            string customerName, string customerAddress1, string customerAddress2, string customerCity, 
            string customerState, string customerPostalCode, string customerCountry, string customerPhone,
            int? customerTypeID, int? customerIndustryID, int? customerAccountTypeID) {

            var account = AppService.Current.Account;
            var customer = new Domain.Companies.Customer();

            if (account.PrimaryCompany.Customers.Where(x => x.Name == customerName).FirstOrDefault() != null) {
                return Content("ERR: This customer name already exists.");
            }

            customer.CompanyID = account.PrimaryCompany.ID;
            customer.Name = customerName;
            customer.Address1 = customerAddress1;
            customer.Address2 = customerAddress2;
            customer.City = customerCity;
            customer.State = customerState;
            customer.PostalCode = customerPostalCode;
            customer.Country = customerCountry;
            customer.Phone = customerPhone;
            customer.TypeID = customerTypeID;
            customer.IndustryID = customerIndustryID;
            customer.AccountTypeID = customerAccountTypeID;
            
            dataContext.Customers.Add(customer);
            dataContext.SaveChanges();

            return Content("ok");
            
        }


        [HttpPost]
        public ActionResult EditCustomer(
            int id, string customerName, string customerAddress1, string customerAddress2, string customerCity,
            string customerState, string customerPostalCode, string customerCountry, string customerPhone,
            int? customerTypeID, int? customerIndustryID, int? customerAccountTypeID) {
            
            var customer = dataContext.Customers.Find(id);

            // verify not duplicate name attempt
            var dummy = AppService.Current.Account.PrimaryCompany.Customers.Where(x => x.Name == customerName).FirstOrDefault();

            if (dummy != null && dummy.ID != id) {
                return Content("This customer name already exists.");
            }
            
            customer.Name = customerName;
            customer.Address1 = customerAddress1;
            customer.Address2 = customerAddress2;
            customer.City = customerCity;
            customer.State = customerState;
            customer.PostalCode = customerPostalCode;
            customer.Country = customerCountry;
            customer.Phone = customerPhone;
            customer.TypeID = customerTypeID;
            customer.IndustryID = customerIndustryID;
            customer.AccountTypeID = customerAccountTypeID;
            
            dataContext.SaveChanges();

            return Content("ok");

        }

        [HttpPost]
        public ActionResult EditCustomerType(int id, string name) {

            var t = dataContext.CustomerTypes.Find(id);

            // verify this type doesn't already exist
            var dummy = AppService.Current.Account.PrimaryCompany.CustomerTypes.Where(x => x.Name == name).FirstOrDefault();

            if (dummy != null && dummy.ID != id) {
                return Content("This customer type already exists");
            }

            t.Name = name;

            dataContext.SaveChanges();

            return Content("ok");

        }

        [HttpPost]
        public ActionResult EditAccountType(int id, string name, int? callsPerMonth) {

            var t = dataContext.AccountTypes.Find(id);

            // verify doesn't already exist
            var dummy = AppService.Current.Account.PrimaryCompany.AccountTypes.Where(x => x.Name == name).FirstOrDefault();

            if (dummy != null && dummy.ID != id) {
                return Content("This account type already exists");
            }

            t.Name = name;
            t.CallsPerMonthGoal = callsPerMonth;

            dataContext.SaveChanges();

            return Content("ok");
        }
        
        [HttpPost]
        public ActionResult DeleteCustomerType(int id) {

            var t = dataContext.CustomerTypes.Find(id);
            dataContext.CustomerTypes.Remove(t);
            dataContext.SaveChanges();

            return Content("ok");

        }
        
        [HttpPost]
        public ActionResult DeleteAccountType(int id) {

            var t = dataContext.AccountTypes.Find(id);
            dataContext.AccountTypes.Remove(t);
            dataContext.SaveChanges();

            return Content("ok");
        }

        [HttpPost]
        public ActionResult EditIndustry(int id, string name) {

            var ind = dataContext.Industries.Find(id);

            // verify this industry doesn't already exist
            var dummy = AppService.Current.Account.PrimaryCompany.Industries.Where(x => x.Name == name).FirstOrDefault();

            if (dummy != null && dummy.ID != id) {
                return Content("This industry already exists - unable to rename");
            }

            ind.Name = name;

            dataContext.SaveChanges();

            return Content("ok");
            
        }

        [HttpPost]
        public ActionResult DeleteIndustry(int id) {

            var ind = dataContext.Industries.Find(id);
            dataContext.Industries.Remove(ind);
            dataContext.SaveChanges();

            return Content("ok");

        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id) {

            var c = dataContext.Customers.Find(id);
            dataContext.Customers.Remove(c);
            dataContext.SaveChanges();

            return Content("ok");
        }
        
        [HttpPost]
        public ActionResult AddCustomerType(string typeName) {

            // make sure the type doesn't already exist for this customer
            var company = AppService.Current.Account.PrimaryCompany;
            var existingType = company.CustomerTypes.Where(x => x.Name == typeName).FirstOrDefault();

            if (existingType != null) {
                return Content("ERR: This type already exists.");
            }

            // other validations??

            // add to EF and save
            dataContext.CustomerTypes.Add(new Domain.Companies.CustomerType()
            {
                CompanyID = company.ID,
                Name = typeName
            });
            dataContext.SaveChanges();

            // return success
            return Content("ok");
        }
        
        [HttpPost]
        public ActionResult AddAccountType(string typeName) {

            // make sure the type doesn't already exist
            var company = AppService.Current.Account.PrimaryCompany;
            var existingType = company.AccountTypes.Where(x => x.Name == typeName).FirstOrDefault();

            if (existingType != null) {
                return Content("ERR: This account type already exists.");
            }

            dataContext.AccountTypes.Add(new Domain.Companies.AccountType()
            {
                CompanyID = company.ID,
                Name = typeName
            });

            dataContext.SaveChanges();

            return Content("ok");
        }
        
        [HttpPost]
        public ActionResult AddCustomerIndustry(string industryName) {

            // make sure the type doesn't already exist for this customer
            var company = AppService.Current.Account.PrimaryCompany;
            var existingIndustry = company.Industries.Where(x => x.Name == industryName).FirstOrDefault();

            if (existingIndustry != null) {
                return Content("ERR: This industry already exists.");
            }

            // other validations??

            // add to EF and save
            dataContext.Industries.Add(new Domain.Companies.Industry()
            {
                CompanyID = company.ID,
                Name = industryName
            });
            dataContext.SaveChanges();

            // return success
            return Content("ok");
        }



        #endregion

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
        CustomersDataService service;

        public CustomersController() {
            dataContext = new Genrev.Data.GenrevContext();
            service = new CustomersDataService(dataContext);
        }


        #endregion




    }
}