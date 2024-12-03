using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Analysis.Models
{
    
    public class HistoricVM
    {

        public List<CommonListItems.YearsToShow> YearsToShowList { get; set; } = CommonListItems.DataService.GetYearsToShowCommonList();
        public List<CommonListItems.Person> PersonsFilterList { get; set; } = CommonListItems.DataService.GetPersonnelCommonList();
        public List<CommonListItems.Industry> IndustriesList { get; set; } = CommonListItems.DataService.GetIndustryCommonList();
        public List<CommonListItems.CustomerType> CustomerTypesList { get; set; } = CommonListItems.DataService.GetCustomerTypeCommonList();
        public List<CommonListItems.AccountType> AccountTypesList { get; set; } = CommonListItems.DataService.GetAccountTypeCommonList();
        public List<CommonListItems.Product> ProductsList { get; set; } = CommonListItems.DataService.GetProductCommonList();
        public List<CommonListItems.Customer> CustomersList { get; set; } = CommonListItems.DataService.GetCustomerCommonList();

        public CommonListItems.YearsToShow DefaultYearsToShow { get; set; }
        public CommonListItems.Person DefaultPerson { get; set; }
        public CommonListItems.Industry DefaultIndustry { get; set; }
        public CommonListItems.CustomerType DefaultCustomerType { get; set; }
        public CommonListItems.AccountType DefaultAccountType { get; set; }
        public CommonListItems.Product DefaultProduct { get; set; }
        public CommonListItems.Customer DefaultCustomer { get; set; }


    }
}