using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.DTOs
{
    class HistoricSales
    {


        public int FiscalYear { get; set; }
        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }
        public decimal? CostActual { get; set; }
        public decimal? CostForecast { get; set; }
        public int? CallsActual { get; set; }
        public int? CallsForecast { get; set; }

    }
}
