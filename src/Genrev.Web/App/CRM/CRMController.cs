using Genrev.Data.DTOs;
using Genrev.Web.App.Customers;
using Genrev.Web.App.Customers.Models;
using Genrev.Web.App.Data;
using Genrev.Web.App.Services;

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Genrev.Web.App.CRM
{
    public class CRMController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {
        private readonly DataService _dataService;
        private readonly CRMService _crmService;
        private readonly CustomersDataService _customerDataservice;

        public CRMController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GenrevContext"].ConnectionString;
            _dataService = new DataService();
            _crmService = new CRMService(connectionString);
            _customerDataservice = new CustomersDataService();
        }

        public ActionResult Index()
        {
            CRMViewModel model = new CRMViewModel();
            model.CRMListItems = _crmService.GetCRMRecords();
            return View(model);
        }

        public ActionResult EditClient(int Id)
        {
            var crmDetails = _crmService.GetCRMDetailsById(Id);
            if (crmDetails != null)
            {
                // Fetch salespersons list
                var salesPersons = CommonListItems.DataService.GetPersonnelCommonList();
                ViewBag.SalesPersons = salesPersons;

                bool IsAdmin = User.IsInRole("sysadmin");
                ViewBag.IsAdmin = IsAdmin;

                // Determine selected salesperson
                //int selectedSalesPersonId = IsAdmin
                //    ? salesPersons.Where(w => w.ID == crmDetails.SalesPersonId).FirstOrDefault()?.ID ?? 0 // If Admin, select the first sales person
                //    : crmDetails.SalesPersonId; // If not Admin, use the CRM record's SalesPersonId
                int selectedSalesPersonId = crmDetails.SalesPersonId;

                ViewBag.SelectedSalesPersonId = selectedSalesPersonId;

                // Get customers assigned to the selected salesperson
                List<CustomerDDLVM> customers = _customerDataservice.GetCustomerListItemsByPersonnelId(selectedSalesPersonId);                

                // Select the CRM record's CustomerId or fallback to the first available customer
                int selectedCustomerId = customers.Any() ? crmDetails.CustomerId : 0;
                ViewBag.SelectedCustomerId = selectedCustomerId;
                ViewBag.Customers = customers;
                crmDetails.Strategy = _crmService.GetStrategyByCustomerId(crmDetails.CustomerId);
                crmDetails.UploadedFiles = _crmService.GetFilesByCRMId(crmDetails.Id);

                var areaOfResponsibilities = _customerDataservice.GetAreaOfResponsibilities(AppService.Current.Account.PrimaryCompany.ID);
                ViewBag.AreaOfResponsibilities = areaOfResponsibilities;
                var selectedAORId = areaOfResponsibilities.Any() ? crmDetails.AreaOfResponsibilityId : 0;
                ViewBag.SelectedAreaOfResponsibilities = selectedAORId;
                return View(crmDetails);
            }

            return RedirectToAction("Index"); // Redirect to Index if CRM details are null
        }

        public ActionResult AddClient()
        {
            var salesPersons = CommonListItems.DataService.GetPersonnelCommonList();
            ViewBag.SalesPersons = salesPersons;
            var areaOfResponsibilities = _customerDataservice.GetAreaOfResponsibilities(AppService.Current.Account.PrimaryCompany.ID);
            ViewBag.AreaOfResponsibilities = areaOfResponsibilities;
            var selectedAORId = areaOfResponsibilities.Any() ? areaOfResponsibilities.First().ID : 0;
            ViewBag.SelectedAreaOfResponsibilities = selectedAORId;

            bool IsAdmin = User.IsInRole("sysadmin");
            ViewBag.IsAdmin = IsAdmin;

            int personnelId = AppService.Current.Person.PersonID;
            int selectedSalesPersonId = IsAdmin
                ? salesPersons.FirstOrDefault()?.ID ?? 0 // Select first sales person if Admin
                : personnelId; // Select logged-in user if SalesPerson

            ViewBag.SelectedSalesPersonId = selectedSalesPersonId;

            List<CustomerDDLVM> customers = _customerDataservice.GetCustomerListItemsByPersonnelId(selectedSalesPersonId);
            ViewBag.Customers = new List<CustomerDDLVM>();
            var selectedCustomerId = customers.Any() ? customers.First().ID : 0;
            ViewBag.SelectedCustomerId = selectedAORId;

            return View(new CRMViewModel { SalesPersonId = selectedSalesPersonId, CustomerId = selectedAORId });
        }

        public JsonResult GetCustomersBySalesPerson(int salesPersonId)
        {
            List<CustomerDDLVM> customers = _customerDataservice.GetCustomerListItemsByPersonnelId(salesPersonId);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Save(CRMViewModel model, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            ModelState.Remove(nameof(model.UploadedFiles));
            ModelState.Remove(nameof(model.CRMListItems));
            if (ModelState.IsValid)
            {
                //var existingData = _crmService.GetCRMRecordsBySalesPersonIdAndCustomerId(model.SalesPersonId, model.CustomerId);
                //if (existingData.Count==0)
                //{
                //}
                // Save the CRM entry and get its ID
                int crmId = _crmService.SaveCRMEntry(model);

                // Save Contacts (Phones, Emails, Notes)
                var contacts = new List<CRMContactViewModel>();

                if (model.Phones != null)
                {
                    contacts.AddRange(model.Phones.Select(phone => new CRMContactViewModel
                    {
                        CRMId = crmId,
                        ContactType = "Phone",
                        ContactValue = phone
                    }));
                }

                if (model.Emails != null)
                {
                    contacts.AddRange(model.Emails.Select(email => new CRMContactViewModel
                    {
                        CRMId = crmId,
                        ContactType = "Email",
                        ContactValue = email
                    }));
                }

                if (model.Notes != null)
                {
                    contacts.AddRange(model.Notes.Select(note => new CRMContactViewModel
                    {
                        CRMId = crmId,
                        ContactType = "Notes",
                        ContactValue = note
                    }));
                }

                // Save all contacts in one call
                if (contacts.Any())
                {
                    _crmService.SaveContacts(crmId, contacts);
                }

                // Save Addresses
                if (model.Addresses != null && model.Addresses.Any())
                {
                    _crmService.SaveAddresses(crmId, model.Addresses);
                }
                // Save uploaded files
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    foreach (var file in uploadedFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {                           
                            var originalFileName = Path.GetFileName(file.FileName);
                            var sanitizedFileName = Regex.Replace(originalFileName, @"[^a-zA-Z0-9_.]+", "_");

                            string path = AppService.Current.Settings.CRMFileUploadDirectory;
                            string absolutePath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + path);

                            if (!Directory.Exists(absolutePath))
                            {
                                Directory.CreateDirectory(absolutePath);
                            }
                            
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                            string extension = Path.GetExtension(sanitizedFileName);
                            string fullPath = Path.Combine(absolutePath, sanitizedFileName);
                            
                            int count = 1;
                            while (System.IO.File.Exists(fullPath))
                            {
                                sanitizedFileName = $"{fileNameWithoutExt}({count}){extension}";
                                fullPath = Path.Combine(absolutePath, sanitizedFileName);
                                count++;
                            }

                            file.SaveAs(fullPath);
                            
                            _crmService.SaveFileMetadata(crmId, sanitizedFileName, Path.Combine(path));
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            // If ModelState is invalid, reload necessary data
            ViewBag.SalesPersons = CommonListItems.DataService.GetPersonnelCommonList();
            ViewBag.IsAdmin = User.IsInRole("sysadmin");
            ViewBag.Customers = _customerDataservice.GetCustomerListItemsByPersonnelId(model.SalesPersonId);

            return View("Index", model);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateClient(CRMViewModel model, IEnumerable<HttpPostedFileBase> uploadedFiles)
        {
            ModelState.Remove(nameof(model.UploadedFiles));
            ModelState.Remove(nameof(model.CRMListItems));
            if (!ModelState.IsValid)
            {
                // Reload the dropdown lists if validation fails
                var salesPersons = CommonListItems.DataService.GetPersonnelCommonList();
                ViewBag.SalesPersons = salesPersons;

                bool IsAdmin = User.IsInRole("sysadmin");
                ViewBag.IsAdmin = IsAdmin;

                int selectedSalesPersonId = IsAdmin
                    ? salesPersons.FirstOrDefault()?.ID ?? 0
                    : model.SalesPersonId;

                ViewBag.SelectedSalesPersonId = selectedSalesPersonId;

                List<CustomerDDLVM> customers = _customerDataservice.GetCustomerListItemsByPersonnelId(selectedSalesPersonId);
                ViewBag.Customers = customers;
                ViewBag.SelectedCustomerId = model.CustomerId;

                return View("EditClient", model);
            }

            // Update CRM entry
            bool isUpdated = _crmService.UpdateCRMEntry(model);
            if (isUpdated)
            {
                if (uploadedFiles != null && uploadedFiles.Any())
                {
                    foreach (var file in uploadedFiles)
                    {
                        if (file != null && file.ContentLength > 0)
                        {                           
                            var originalFileName = Path.GetFileName(file.FileName);
                            var sanitizedFileName = Regex.Replace(originalFileName, @"[^a-zA-Z0-9_.]+", "_");

                            string path = AppService.Current.Settings.CRMFileUploadDirectory;
                            string absolutePath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + path);

                            if (!Directory.Exists(absolutePath))
                            {
                                Directory.CreateDirectory(absolutePath);
                            }

                            // Split filename and extension
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
                            string extension = Path.GetExtension(sanitizedFileName);
                            string fullPath = Path.Combine(absolutePath, sanitizedFileName);

                            // Append number if file exists
                            int count = 1;
                            while (System.IO.File.Exists(fullPath))
                            {
                                sanitizedFileName  = $"{fileNameWithoutExt}({count}){extension}";
                                fullPath = Path.Combine(absolutePath, sanitizedFileName);
                                count++;
                            }
                            file.SaveAs(fullPath);

                            // Save sanitized file name and path to the database
                            _crmService.SaveFileMetadata(model.Id, sanitizedFileName, Path.Combine(path));
                        }
                    }
                }
                return RedirectToAction("Index");
            }

            // Reload dropdowns if update fails
            var allSalesPersons = CommonListItems.DataService.GetPersonnelCommonList();
            ViewBag.SalesPersons = allSalesPersons;
            ViewBag.SelectedSalesPersonId = model.SalesPersonId;

            List<CustomerDDLVM> allCustomers = _customerDataservice.GetCustomerListItemsByPersonnelId(model.SalesPersonId);
            ViewBag.Customers = allCustomers;
            ViewBag.SelectedCustomerId = model.CustomerId;

            ModelState.AddModelError("", "Failed to update the client details. Please try again.");
            return View("EditClient", model);
        }

        [HttpGet]
        public JsonResult GetStrategyByCustomer(int customerId)
        {
            string strategy = _crmService.GetStrategyByCustomerId(customerId);
            return Json(new { strategy }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteFile(int Id)
        {
            _crmService.DeleteFile(Id);
            return Json(new { success = true });
        }
        public ActionResult GetFilesByCRMId(int crmId)
        {
            var files = _crmService.GetFilesByCRMId(crmId);
            return Json(files, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CRMGridCallback()
        {
            var model = new CRMViewModel();
            model.CRMListItems = _crmService.GetCRMRecords();
            return PartialView("CRMGrid", model);
        }
        [HttpGet]
        public ActionResult DeleteClient(int Id)
        {
            _crmService.DeleteCRMRecord(Id);
            return RedirectToAction("Index");
        }
    }
}
