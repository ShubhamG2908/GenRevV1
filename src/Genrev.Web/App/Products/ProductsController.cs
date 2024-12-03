using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc.DevExpress;
using Genrev.Domain;
using Genrev.Web.App.Personnel.Models;


namespace Genrev.Web.App.Products
{
    [Authorize(Roles = "sysadmin")]
    public class ProductsController : ContentAreaController
    {

        public ActionResult Index() {
            if (!AppService.Current.Settings.ProductFeatureEnabled)
            {
                return GetView("FeatureDisabled");
            }
            var companyID = AppService.Current.Account.PrimaryCompany.ID;
            var model = _service.GetProductsVM(companyID);    
            return GetView("Index", model);
        }

        public ActionResult AddNewGroupPopup() {
            var model = new Models.ProductGroupListItem();
            ViewBag.ProductGroupsList = _service.GetProductGroupsList(AppService.Current.Account.PrimaryCompany.ID);
            return PartialView("AddProductGroupPopup", model);
        }

        public ActionResult AddNewPopup() {
            ViewBag.ProductGroupsList = _service.GetProductGroupsList(AppService.Current.Account.PrimaryCompany.ID);
            return PartialView("AddProductPopup");
        }


        [HttpPost]
        public ActionResult AddProduct(string sku, string description, int? groupID) {

            var thisCompany = AppService.Current.Account.PrimaryCompany;

            thisCompany.Products.Add(new Domain.Products.Product()
            {
                CompanyID = thisCompany.ID,
                Description = description,
                SKU = sku,
                ProductGroupID = groupID
            });

            AppService.Current.DataContext.SaveChanges();

            return Content("ok");
        }

        [HttpPost]
        public ActionResult AddGroup(string groupName, int? parentID) {

            var thisCompany = AppService.Current.Account.PrimaryCompany;

            thisCompany.ProductGroups.Add(new Domain.Products.ProductGroup()
            {
                CompanyID = thisCompany.ID,
                Name = groupName,
                ParentID = parentID
            });

            AppService.Current.DataContext.SaveChanges();

            return Content("ok");
        }

        [HttpPost]
        public ActionResult RemoveGroup(int groupID) {

            _service.RemoveProductGroup(groupID);

            return Content("ok");
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id) {

            var prod = AppService.Current.DataContext.Products.Find(id);
            AppService.Current.DataContext.Products.Remove(prod);
            AppService.Current.DataContext.SaveChanges();

            return Content("ok");
        }
        


       [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ProductGroupGridCallback() {
            var model = _service.GetProductGroupsList(AppService.Current.Account.PrimaryCompany.ID);
            return PartialView("ProductGroupGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ProductsGridCallback() {
            var model = _service.GetProductsList(AppService.Current.Account.PrimaryCompany.ID);
            return PartialView("ProductsGrid", model);
        }











        private ProductsService _service;

        public ProductsController() {
            _service = new ProductsService();
        }


    }
}