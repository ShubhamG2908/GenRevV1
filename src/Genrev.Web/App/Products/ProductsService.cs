using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Web.App.Products.Models;

namespace Genrev.Web.App.Products
{
    public class ProductsService
    {



        internal List<ProductGroupListItem> GetProductGroupsList(int companyID) {

            var groups = _context.ProductGroups.Where(x => x.CompanyID == companyID);

            var model = new List<ProductGroupListItem>();

            foreach (var g in groups) {
                model.Add(new ProductGroupListItem()
                {
                    ID = g.ID,
                    Name = g.Name,
                    ParentGroupID = g.ParentID,
                    ParentGroup = g.Parent?.Name
                });
            }

            return model;

        }



        internal List<ProductListItem> GetProductsList(int companyID) {

            var products = _context.Products.Where(x => x.CompanyID == companyID).ToList();

            var model = new List<ProductListItem>();

            foreach (var p in products) {

                model.Add(new ProductListItem()
                {
                    ID = p.ID,
                    SKU = p.SKU,
                    Description = p.Description,
                    ProductGroupID = p.ProductGroupID,
                    Group = p.ProductGroup?.Name
                });

            }

            return model;
        }


        internal ProductsVM GetProductsVM(int companyID) {
            
            
            var model = new ProductsVM();

            model.Products = this.GetProductsList(companyID);
            model.ProductGroups = this.GetProductGroupsList(companyID);
            
            return model;
        }

        internal void RemoveProductGroup(int groupID) {

            // remove associations from parent group references, procuct references, then finally the group iteself

            var associatedGroups = _context.ProductGroups.Where(x => x.ParentID == groupID).ToList();
            foreach (var g in associatedGroups) {
                g.ParentID = null;
            }

            var associatedProducts = _context.Products.Where(x => x.ProductGroupID == groupID).ToList();
            foreach (var p in associatedProducts) {
                p.ProductGroupID = null;
            }

            var group = _context.ProductGroups.Find(groupID);
            _context.ProductGroups.Remove(group);

            _context.SaveChanges();
        }

        private Genrev.Data.GenrevContext _context;

        public ProductsService() {
            this._context = AppService.Current.DataContext;
        }

        
    }
}