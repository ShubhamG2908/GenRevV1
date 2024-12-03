using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Web.App.Customers.Models;

namespace Genrev.Web.App.Customers
{
    public class CustomersDataService
    {


        public List<CustomerListItemVM> GetCustomerListItems() {

            var items = new List<Models.CustomerListItemVM>();
            var account = AppService.Current.Account;
            var domainCustomers = account.PrimaryCompany.Customers.OrderBy(x => x.Name).ToList();

            foreach (var dc in domainCustomers) {

                var c = new CustomerListItemVM();
                c.ID = dc.ID;
                c.Industry = dc.Industry?.Name;
                c.Address1 = dc.Address1;
                c.City = dc.City;
                c.State = dc.State;
                c.PostalCode = dc.PostalCode;
                c.Country = dc.Country;
                c.Phone = dc.Phone;
                c.Name = dc.Name;
                c.CustomerType = dc.Type?.Name;
                c.AccountType = dc.AccountType?.Name;                

                items.Add(c);
            }

            return items;
        }


        public List<MappingCustomerListItemVM> GetCustomerMappingsListItems(int personnelID) {

            var model = new List<Models.MappingCustomerListItemVM>();

            var account = AppService.Current.Account;
            var person = account.PrimaryCompany.Personnel.Where(x => x.ID == personnelID).Single();
            var domainCustomers = account.PrimaryCompany.Customers.ToList();

            var mappedCustomers = account.PrimaryCompany.Customers.Where(x => x.Personnel.Contains(person)).ToList();

            foreach (var dc in domainCustomers) {
                var c = new Models.MappingCustomerListItemVM();
                c.ID = dc.ID;
                c.Name = dc.Name;
                if (mappedCustomers.Contains(dc)) {
                    c.Selected = true;
                }
                model.Add(c);
            }

            return model;
            
        }

        public void ToggleCustomerMapping(int personnelID, int customerID) {

            var person = dataContext.Personnel.Find(personnelID);
            var existingCustomer = person.Customers.Where(x => x.ID == customerID).FirstOrDefault();

            if (existingCustomer == null) {
                // toggle on
                var newCustomer = dataContext.Customers.Find(customerID);
                person.Customers.Add(newCustomer);
            } else {
                // toggle off
                person.Customers.Remove(existingCustomer);
            }

            dataContext.SaveChanges();

        }






        private Genrev.Data.GenrevContext dataContext;

        public CustomersDataService() {
            dataContext = new Genrev.Data.GenrevContext();
        }

        public CustomersDataService(Genrev.Data.GenrevContext context) {
            dataContext = context;
        }


    }
}