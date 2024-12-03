using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class CustomerDrilldown : DataAggregateBase {

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

    }
}
