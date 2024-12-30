using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.DTOs
{

    [Obsolete("Use Genrev.Data.DTOs.MonthlyData")]
    class MonthlyDataByPersonnel
    {
        public DateTime Period { get; set; }
        public decimal? DataSalesActualSum { get; set; }
        public decimal? DataSalesForecastSum { get; set; }
        public decimal? DataCostActualSum { get; set; }
        public decimal? DataCostForecastSum { get; set; }
        public double? DataCallsActualSum { get; set; }
        public double? DataCallsForecastSum { get; set; }
    }
}
