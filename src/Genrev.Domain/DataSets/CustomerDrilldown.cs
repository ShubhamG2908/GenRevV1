using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class CustomerDrilldown : DataAggregateBase
    {

        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int? PersonnelID { get; set; }
        public int? ProductID { get; set; }

        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }

        public DateTime Period { get; set; }
        public int? CalendarYear { get; set; }
        public int? CalendarMonth { get; set; }
        public int? FiscalYear { get; set; }

        public int? IndustryID { get; set; }
        public int? CustomerTypeID { get; set; }
        public int? AccountTypeID { get; set; }

        public string IndustryName { get; set; }
        public string CustomerTypeName { get; set; }
        public string AccountTypeName { get; set; }

        public string ProductSKU { get; set; }
        public string ProductDescription { get; set; }
        public int? ProductGroupID { get; set; }
        public string ProductGroupName { get; set; }
        public string CustomerName { get; set; }


        public virtual Companies.Customer Customer { get; set; }
        public virtual Companies.Person Person { get; set; }
        public virtual Products.Product Product { get; set; }

        public virtual Companies.Industry Industry { get; set; }
        public virtual Companies.CustomerType CustomerType { get; set; }
        public virtual Companies.AccountType AccountType { get; set; }

        public CustomerDrilldown() { }

        public CustomerDrilldown(CustomerDrilldownVM vm)
        {
            Period = vm.Period;
            CalendarMonth = vm.CalendarMonth;
            CalendarYear = vm.CalendarYear;
            CustomerName = vm.CustomerName;
            PersonnelID = vm.PersonnelID;
            PersonFirstName = vm.PersonFirstName;
            PersonLastName = vm.PersonLastName;
            ProductSKU = vm.ProductSKU;
            IndustryID = vm.IndustryID;
            CustomerTypeID = vm.CustomerTypeID;
            AccountTypeID = vm.AccountTypeID;
            IndustryName = vm.IndustryName;
            CustomerTypeName = vm.CustomerTypeName;
            AccountTypeName = vm.AccountTypeName;
            SalesActual = vm.SalesActual;
            SalesForecast = vm.SalesForecast;
            CostActual = vm.CostActual;
            CostForecast = vm.CostForecast;
            CallsActual = vm.CallsActual;
            CallsForecast = vm.CallsForecast;
        }
    }

    public class CustomerDrilldownVM
    {
        public DateTime Period => new DateTime((int)CalendarYear, (int)CalendarMonth, 1);
        public int? CalendarYear { get; set; }
        public int? CalendarMonth { get; set; }

        public string CustomerName { get; set; }

        public int? PersonnelID { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string ProductSKU { get; set; }

        public int? IndustryID { get; set; }
        public int? CustomerTypeID { get; set; }
        public int? AccountTypeID { get; set; }

        public string IndustryName { get; set; }
        public string CustomerTypeName { get; set; }
        public string AccountTypeName { get; set; }

        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }

        public decimal? CostActual { get; set; }
        public decimal? CostForecast { get; set; }

        public int? CallsActual { get; set; }
        public int? CallsForecast { get; set; }

    }
}
