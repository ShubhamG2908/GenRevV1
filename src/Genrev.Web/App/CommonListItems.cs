using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App
{
    public class CommonListItems
    {

        public class Industry
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class CustomerType
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class AccountType
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        public class YearsToShow
        {
            public int Number { get; set; }
            public string Display { get; set; }
        }

        public class FiscalYear
        {
            public int Number { get; set; }
        }


        public class Period
        {
            public DateTime Date { get; set; }
            public string DisplayShort {
                get
                {
                    return Date.ToString("MMMM");
                }
            }
        }
        
        public class Person
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get 
                {
                    var firstName = FirstName;
                    var lastName = LastName;

                    if (firstName != null)
                        firstName.Trim();

                    if (lastName != null)
                        lastName.Trim();

                    var displayName = firstName + " " + lastName;
                    return displayName.Trim();
                } 
            }
        }


        public class Product
        {
            public int ID { get; set; }
            public string SKU { get; set; }
            public int? GroupID { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
        }

        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }



        public static class DataService
        {


            /// <summary>
            /// Retrieves a list of fiscal years for which the current account context has data (min/max and all inclusive), or null if no data found.
            /// </summary>
            /// <returns>List of FiscalYears for display (not to be confused with Domain.FiscalYear)</returns>
            public static List<FiscalYear> GetFiscalYearList() {

                var context = AppService.Current.DataContext;
                var companyID = AppService.Current.Account.PrimaryCompany.ID;

                var q = from cd in context.CustomerData
                        join cust in context.Customers on cd.CustomerID equals cust.ID
                        where cust.CompanyID == companyID
                        group new { cd.Period } by cust.CompanyID into g
                        select new MinMaxDateDTO
                        {
                            Min = g.Min(x => x.Period),
                            Max = g.Max(x => x.Period)
                        };

                MinMaxDateDTO mmd = q.SingleOrDefault();

                if (mmd == null) {
                    return null;
                }

                int min = mmd.Min.Year;
                int max = mmd.Max.Year;

                var items = new List<FiscalYear>();

                for (int i = min; i <= max; i++) {
                    items.Add(new FiscalYear() { Number = i });
                }

                return items.OrderByDescending(x => x.Number).ToList();
            }


            private class MinMaxDateDTO
            {
                public DateTime Min { get; set; }
                public DateTime Max { get; set; }
            }


            public static List<Industry> GetIndustryCommonList() {

                var data = AppService.Current.Account.PrimaryCompany.Industries.OrderBy(x => x.Name).ToList();
                var items = new List<Industry>();

                foreach (var d in data) {
                    items.Add(new Industry() { ID = d.ID, Name = d.Name });
                }

                return items;
                
            }

            public static List<CustomerType> GetCustomerTypeCommonList() {
                
                var data = AppService.Current.Account.PrimaryCompany.CustomerTypes.OrderBy(x => x.Name).ToList();
                var items = new List<CustomerType>();

                foreach (var d in data) {
                    items.Add(new CustomerType() { ID = d.ID, Name = d.Name });
                }

                return items;

            }

            public static List<AccountType> GetAccountTypeCommonList() {

                var data = AppService.Current.Account.PrimaryCompany.AccountTypes.OrderBy(x => x.Name).ToList();
                var items = new List<AccountType>();

                foreach (var d in data) {
                    items.Add(new AccountType() { ID = d.ID, Name = d.Name });
                }

                return items;

            }

            public static List<YearsToShow> GetYearsToShowCommonList() {

                var items = new List<YearsToShow>();

                items.Add(new YearsToShow() { Number = 1, Display = "1 Year" });
                items.Add(new YearsToShow() { Number = 2, Display = "2 Years" });
                items.Add(new YearsToShow() { Number = 3, Display = "3 Years" });
                items.Add(new YearsToShow() { Number = 4, Display = "4 Years" });
                items.Add(new YearsToShow() { Number = 5, Display = "5 Years" });
                items.Add(new YearsToShow() { Number = 7, Display = "7 Years" });
                items.Add(new YearsToShow() { Number = 10, Display = "10 Years" });
                items.Add(new YearsToShow() { Number = 15, Display = "15 Years" });

                return items;
            }

            public static List<Period> GetPeriodCommonList(List<DateTime> fyPeriods) {
                var items = new List<Period>();
                foreach (var d in fyPeriods) {
                    items.Add(new Period() { Date = d });
                }
                return items;
            }


            // not restricted by logged in user
            public static List<Person> GetPersonnalCommonListAll() {

                var context = AppService.Current.DataContext;
                var personnel = context.Personnel.ToList();
                var items = new List<Person>();

                foreach (var p in personnel) {
                    items.Add(new Person()
                    {
                        ID = p.ID,
                        FirstName = p.FirstName,
                        LastName = p.LastName
                    });
                }

                return items;
            }


            public static List<Person> GetPersonnelCommonList() {
                return GetPersonnelCommonList(AppService.Current.Person.PersonID);
            }

            public static List<Person> GetPersonnelCommonList(int personID) {
                
                var context = AppService.Current.DataContext;
                var personnel = context.GetDownstreamPersonnel(personID);
                var items = new List<Person>();

                foreach (var p in personnel) {
                    items.Add(new Person()
                    {
                        ID = p.ID,
                        FirstName = p.FirstName,
                        LastName = p.LastName
                    });
                }

                return items;
            }


            public static List<Product> GetProductCommonList() {

                var context = AppService.Current.DataContext;
                var products = context.Products.Where(x => x.CompanyID == AppService.Current.Account.PrimaryCompany.ID).ToList();
                var items = new List<Product>();

                foreach (var p in products) {
                    items.Add(new Product()
                    {
                        Description = p.Description,
                        GroupID = p.ProductGroupID,
                        GroupName = p.ProductGroup?.Name,
                        ID = p.ID,
                        SKU = p.SKU
                    });
                }

                return items;
            }

            public static List<Customer> GetCustomerCommonList() {

                var context = AppService.Current.DataContext;
                var customers = context.Customers.Where(x => x.CompanyID == AppService.Current.Account.PrimaryCompany.ID);
                var items = new List<Customer>();

                foreach (var c in customers) {
                    items.Add(new Customer()
                    {
                        ID = c.ID,
                        Name = c.Name
                    });
                }

                return items;
            }

        }

    }
}