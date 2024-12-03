using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class CustomerData
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CustomerID { get; set; }
        public int? PersonnelID { get; set; }
        public int? ProductID { get; set; }

        public DateTime Period { get; set; }

        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? SalesTarget { get; set; }

        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }

        public decimal? CostActual { get; set; }
        public decimal? CostForecast { get; set; }
        public decimal? CostTarget { get; set; }

        public int? CallsActual { get; set; }
        public int? CallsForecast { get; set; }
        public int? CallsTarget { get; set; }

        public string Strategy { get; set; }

        public virtual Companies.Customer Customer { get; set; }
        public virtual Companies.Person Person { get; set; }
        public virtual Products.Product Product { get; set; }


        public static decimal? GetCost(decimal? sales, decimal? gpp)
        {
            return sales * (1 - gpp / 100);
        }

        public static decimal? GetGPP(decimal? sales, decimal? cost)
        {
            return sales == 0 ? 0 : (1 - cost / sales) * 100;
            //return (d.SalesTarget - d.CostTarget) / d.SalesTarget * 100;
        }
    }
}
