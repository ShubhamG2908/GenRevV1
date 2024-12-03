using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public abstract class DataAggregateBase
    {


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

        
        public int? CallsDifference {
            get
            {
                if (!CallsActual.HasValue) {
                    CallsActual = 0;
                }
                if (!CallsForecast.HasValue)
                {
                    CallsForecast = 0;
                }
                return CallsActual.Value - CallsForecast.Value;
            }
        }


        public decimal? GrossProfitPercentDifference {
            get
            {
                decimal grossProfitPercent;
                decimal grossProfitPercentForecast;

                if (!GrossProfitPercent.HasValue && !GrossProfitPercentForecast.HasValue)
                {
                    return null;
                }

                if (!GrossProfitPercent.HasValue) {
                    grossProfitPercent = 0;
                }
                else
                {
                    grossProfitPercent = GrossProfitPercent.Value;
                }
                if (!GrossProfitPercentForecast.HasValue)
                {
                    grossProfitPercentForecast = 0;
                }
                else
                {
                    grossProfitPercentForecast = GrossProfitPercentForecast.Value;
                }

                decimal result = (grossProfitPercent - grossProfitPercentForecast) / 100;
                return Math.Round(result, 4, MidpointRounding.AwayFromZero);
            }
        }

        public decimal? GrossProfitPercentVariance {
            get
            {
                if (!GrossProfitPercentForecast.HasValue) {
                    return null;
                }
                if (GrossProfitPercentForecast.Value == 0) {
                    return null;
                }
                return (GrossProfitPercentDifference.Value / GrossProfitPercentForecast.Value) * 100;
            }
        }

        public decimal? GrossProfitDollarsDifference {
            get
            {
                decimal grossProfitDollars;
                decimal grossProfitDollarsForecast;

                if (!GrossProfitDollars.HasValue && !GrossProfitDollarsForecast.HasValue)
                {
                    return null;
                }

                if (!GrossProfitDollars.HasValue) {
                    grossProfitDollars = 0;
                }
                else
                {
                    grossProfitDollars = GrossProfitDollars.Value;
                }
                if (!GrossProfitDollarsForecast.HasValue)
                {
                    grossProfitDollarsForecast = 0;
                }
                else
                {
                    grossProfitDollarsForecast = GrossProfitDollarsForecast.Value;
                }
                return grossProfitDollars - grossProfitDollarsForecast;
            }
        }

        public decimal? GrossProfitDollarsVariance {
            get
            {
                if (!GrossProfitDollarsForecast.HasValue) {
                    return null;
                }
                if (GrossProfitDollarsForecast.Value == 0)
                {
                    return null;
                }
                return GrossProfitDollarsDifference.Value / GrossProfitDollarsForecast.Value;
            }
        }


        public decimal? SalesDifference {
            get
            {
                if (!SalesActual.HasValue && !SalesForecast.HasValue) {
                    return null;
                }
                if (!SalesActual.HasValue)
                {
                    SalesActual = 0;
                }
                if (!SalesForecast.HasValue)
                {
                    SalesForecast = 0;
                }
                return SalesActual.Value - SalesForecast.Value;
            }
        }

        public decimal? SalesVariance {
            get
            {
                if (!SalesForecast.HasValue)
                {
                    return null;
                }
                if (SalesForecast.Value == 0)
                {
                    return null;
                }
                return SalesDifference.Value / SalesForecast.Value;
            }
        }

        public decimal? GrossProfitDollars {
            get
            {
                return SalesActual - CostActual;
            }
        }

        public decimal? GrossProfitDollarsForecast {
            get
            {
                return SalesForecast - CostForecast;
            }
        }

        public decimal? GrossProfitDollarsTarget
        {
            get
            {
                return SalesTarget - CostTarget;
            }
        }

        public decimal? GrossProfitPercent {
            get
            {
                try {
                    return (SalesActual - CostActual) / SalesActual * 100;
                }
                catch (DivideByZeroException) {
                    return 0;
                }
            }
        }

        public decimal? GrossProfitPercentForecast {
            get
            {
                try {
                    return (SalesForecast - CostForecast) / SalesForecast * 100;
                }
                catch (DivideByZeroException) {
                    return 0;
                }
            }
        }

        public decimal? GrossProfitPercentTarget
        {
            get
            {
                try
                {
                    return (SalesTarget - CostTarget) / SalesTarget * 100;
                }
                catch (DivideByZeroException)
                {
                    return 0;
                }
            }
        }

    }
}
