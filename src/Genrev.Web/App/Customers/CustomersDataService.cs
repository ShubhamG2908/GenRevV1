using Dapper;

using Genrev.Web.App.Customers.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Genrev.Web.App.Customers
{
    public class CustomersDataService
    {


        public List<CustomerListItemVM> GetCustomerListItems()
        {

            var items = new List<Models.CustomerListItemVM>();
            var account = AppService.Current.Account;
            var domainCustomers = account.PrimaryCompany.Customers.OrderBy(x => x.Name).ToList();

            foreach (var dc in domainCustomers)
            {

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

        public List<MappingCustomerListItemVM> GetCustomerMappingsListItems(int personnelID)
        {

            var model = new List<Models.MappingCustomerListItemVM>();

            var account = AppService.Current.Account;
            var person = account.PrimaryCompany.Personnel.Where(x => x.ID == personnelID).Single();
            var domainCustomers = account.PrimaryCompany.Customers.ToList();

            var mappedCustomers = account.PrimaryCompany.Customers.Where(x => x.Personnel.Contains(person)).ToList();

            foreach (var dc in domainCustomers)
            {
                var c = new Models.MappingCustomerListItemVM();
                c.ID = dc.ID;
                c.Name = dc.Name;
                if (mappedCustomers.Contains(dc))
                {
                    c.Selected = true;
                }
                model.Add(c);
            }
            return model;
        }
        public List<CustomerDDLVM> GetCustomerListItemsByPersonnelId(int personnelID)
        {
            var model = new List<CustomerDDLVM>();
            //var persons = AppService.Current.Person.Personnel;
            //var account = AppService.Current.Account;

            var customerIDs = AppService.Current.DataContext.GetDownstreamCustomerIDs(personnelID);
            var mappedCustomers = AppService.Current.DataContext.Customers.Where(c => customerIDs.Any(z => z == c.ID)).ToList();

            //var person = account.PrimaryCompany.Personnel.Where(x => x.ID == personnelID).Single();
            //var mappedCustomers = personnelID == 1 ? account.PrimaryCompany.Customers.ToList() : account.PrimaryCompany.Customers.Where(x => x.Personnel.Contains(person)).ToList();
            foreach (var dc in mappedCustomers)
            {
                var c = new Models.CustomerDDLVM();
                c.ID = dc.ID;
                c.Name = dc.Name;
                model.Add(c);
            }
            return model;
        }

        public void ToggleCustomerMapping(int personnelID, int customerID)
        {

            var person = dataContext.Personnel.Find(personnelID);
            var existingCustomer = person.Customers.Where(x => x.ID == customerID).FirstOrDefault();

            if (existingCustomer == null)
            {
                // toggle on
                var newCustomer = dataContext.Customers.Find(customerID);
                person.Customers.Add(newCustomer);
            }
            else
            {
                // toggle off
                person.Customers.Remove(existingCustomer);

                //delete existing records from CustomerData table for this personnelID and CustomerID related.
                var existingCustomerData = dataContext.CustomerData.Where(w => w.PersonnelID == personnelID && w.CustomerID == customerID).ToList();
                if (existingCustomerData.Count > 0)
                {
                    dataContext.CustomerData.RemoveRange(existingCustomerData);
                }
            }

            dataContext.SaveChanges();

        }






        private Genrev.Data.GenrevContext dataContext;

        public CustomersDataService()
        {
            dataContext = new Genrev.Data.GenrevContext();
        }

        public CustomersDataService(Genrev.Data.GenrevContext context)
        {
            dataContext = context;
        }


        public List<Models.AreaOfResponsibilityListItemVM> GetAreaOfResponsibilities(int companyID)
        {
            var model = new List<Models.AreaOfResponsibilityListItemVM>();
            var areaofresponsibilities = dataContext.CompanyAreaOfResponsibilities.Where(x => x.CompanyID == companyID).ToList();
            foreach (var ind in areaofresponsibilities)
            {
                model.Add(new Models.AreaOfResponsibilityListItemVM()
                {
                    ID = ind.ID,
                    Name = ind.Name
                });
            }
            return model.ToList();
        }
        public AreaOfResponsibilityListItemVM DetailAreaOfResponsibilities(int id)
        {
            var area = dataContext.CompanyAreaOfResponsibilities.Find(id);
            if (area == null) throw new Exception("Not found.");
            return new AreaOfResponsibilityListItemVM()
            {
                ID = area.ID,
                Name = area.Name
            };
        }
        public AreaOfResponsibilityListItemVM DetailAreaOfResponsibilitiesByName(string name)
        {
            var area = dataContext.CompanyAreaOfResponsibilities.Where(w => w.Name == name).FirstOrDefault();
            if (area == null) return null;
            return new AreaOfResponsibilityListItemVM()
            {
                ID = area.ID,
                Name = area.Name
            };
        }
        public void AddAreaOfResponsibility(string name, int companyID)
        {
            var existing = dataContext.CompanyAreaOfResponsibilities.FirstOrDefault(x => x.Name == name && x.CompanyID == companyID);
            if (existing != null)
            {
                throw new Exception("This area of responsibility already exists.");
            }
            dataContext.CompanyAreaOfResponsibilities.Add(new Domain.Companies.AreaOfResponsibility
            {
                CompanyID = companyID,
                Name = name
            });
            dataContext.SaveChanges();
        }

        public void EditAreaOfResponsibility(int id, string name)
        {
            var area = dataContext.CompanyAreaOfResponsibilities.Find(id);
            if (area == null) throw new Exception("Not found.");
            area.Name = name;
            dataContext.SaveChanges();
        }

        public int DeleteAreaOfResponsibility(int id)
        {          
            string connectionString = ConfigurationManager.ConnectionStrings["GenrevContext"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT COUNT(1) FROM CRM WHERE AreaOfResponsibilityID = @AreaOfResponsibilityID";

                int recordCount = connection.ExecuteScalar<int>(query, new { AreaOfResponsibilityID = id });
                if (recordCount > 0)
                {
                    return recordCount;
                }
            }

            var area = dataContext.CompanyAreaOfResponsibilities.Find(id);
            if (area == null) throw new Exception("Not found.");
            dataContext.CompanyAreaOfResponsibilities.Remove(area);
            dataContext.SaveChanges();
            return 0;
        }
    }
}