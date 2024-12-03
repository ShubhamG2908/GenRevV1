using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain
{


    public class FiscalYear
    {

        public const int NUMBER_OF_MONTHS = 12;


        #region Properties & Fields


        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public FiscalQuarter Q1 { get; set; }
        public FiscalQuarter Q2 { get; set; }
        public FiscalQuarter Q3 { get; set; }
        public FiscalQuarter Q4 { get; set; }

        public List<DateTime> MonthlyPeriods
        {
            get
            {
                var items = new List<DateTime>();
                var d = this.StartDate;
                while (d <= EndDate)
                {
                    items.Add(d);
                    d = d.AddMonths(1);
                }
                return items;
            }
        }

        public string GetMonthName(int month)
        {

            return MonthlyPeriods[month - 1].ToString("MMMM");
        }

        public string GetMonthShortName(int month)
        {

            return MonthlyPeriods[month - 1].ToString("MMM");
        }



        public List<DataSets.MonthlyData> MonthlyDataByPersonnel { get; set; }

        public int DaysInYear
        {
            get
            {
                return (int)((EndDate - StartDate).TotalDays);
            }
        }


        #endregion



        #region Static Methods


        public static FiscalYear GetByYear(int year, Month fiscalYearEndingMonth)
        {

            // get a reference date that we know is within the target FY (first day of ending month of year should work)
            var refDate = new DateTime(year, (int)fiscalYearEndingMonth, 1);

            // return the current based on our refdate
            return GetCurrent(refDate, fiscalYearEndingMonth);
        }

        public static FiscalYear GetCurrent(DateTime currentDate, Month fiscalYearEndingMonth)
        {

            var currentMonth = currentDate.Month;
            var endingMonth = (int)fiscalYearEndingMonth;

            var currMonth = currentDate.Month;

            // loop the months forward until we hit the ending month
            var countingDate = currentDate;
            while (countingDate.Month < endingMonth)
            {
                countingDate = countingDate.AddMonths(1);
            }

            if (currMonth > endingMonth)
            {
                countingDate = countingDate.AddYears(1);
            }

            var fyEnd = new DateTime(countingDate.Year, endingMonth, 1).AddMonths(1).AddDays(-1);
            var fyStart = fyEnd.AddYears(-1).AddDays(1);

            return new FiscalYear()
            {
                StartDate = fyStart,
                EndDate = fyEnd
            };
        }

        #endregion




        #region Year End Forecasts

        /// <summary>
        /// Calculate the "Accurate" YE Forecast based on current Sales To Date, plus future forecast
        /// (as opposed to simply summing all forecast amounts for the year)
        /// </summary>
        /// <param name="currentDate">Reference date to use for determining the current month</param>
        /// <returns></returns>
        [Obsolete("This method is depracated - do not use.  Functionality is realized via other means now")]
        public decimal? GetAccurateYearEndForecast(DateTime currentDate)
        {

            if (MonthlyDataByPersonnel == null || MonthlyDataByPersonnel.Count < NUMBER_OF_MONTHS)
            {
                throw new InvalidOperationException("SalesByMonth must be supplied to this class to run this method");
            }

            decimal? ytdSales = MonthlyDataByPersonnel.Sum(x => x.SalesActual);

            DataSets.MonthlyData currentMonthData = MonthlyDataByPersonnel.Where(x => x.Period.Month == currentDate.Month).Single();

            decimal? currentMonthRemaingForecast = 0;

            if (currentMonthData.SalesActual > currentMonthData.SalesForecast)
            {
                currentMonthRemaingForecast = currentMonthData.SalesActual;
            }
            else
            {
                currentMonthRemaingForecast = currentMonthData.SalesForecast - currentMonthData.SalesActual;
            }

            decimal? futureMonthsForecast = MonthlyDataByPersonnel.Where(x => x.Period > currentDate).Sum(x => x.SalesForecast);

            return ytdSales + currentMonthRemaingForecast + futureMonthsForecast;

        }


        /// <summary>
        /// Calculate the "Accurate" YE Forecast based on current GPP To Date, plus future GPD forecast
        /// (as opposed to simply summing all forecast GPP amounts for the year)
        /// </summary>
        /// <param name="currentDate">Reference date to use for determining the current month</param>
        /// <returns></returns>
        public decimal? GetAccurateYearEndGPPForecast(DateTime currentDate)
        {

            decimal? sales = GetAccurateYearEndForecast(currentDate);
            decimal? gp = GetAccurateYearEndGPDForecast(currentDate);

            return (decimal?)((((sales - gp) / sales) - 1) * -1 * 100);
        }


        /// <summary>
        /// Calculate the "Accurate" YE Forecast based on current GPD To Date, plus future GPD forecast
        /// (as opposed to simply summing all forecast GPD amounts for the year)
        /// </summary>
        /// <param name="currentDate">Reference date to use for determining the current month</param>
        /// <returns></returns>
        public decimal? GetAccurateYearEndGPDForecast(DateTime currentDate)
        {

            if (MonthlyDataByPersonnel == null || MonthlyDataByPersonnel.Count < NUMBER_OF_MONTHS)
            {
                throw new InvalidOperationException("SalesByMonth must be supplied to this class to run this method");
            }

            decimal? ytdGPD = MonthlyDataByPersonnel.Sum(x => x.GrossProfitDollars);

            DataSets.MonthlyData currentMonthData = MonthlyDataByPersonnel.Where(x => x.Period.Month == currentDate.Month).Single();

            decimal? currentMonthRemaingForecast = 0;

            if (currentMonthData.GrossProfitDollars > currentMonthData.GrossProfitDollarsForecast)
            {
                currentMonthRemaingForecast = currentMonthData.GrossProfitDollars;
            }
            else
            {
                currentMonthRemaingForecast = currentMonthData.GrossProfitDollarsForecast - currentMonthData.GrossProfitDollars;
            }

            decimal? futureMonthsForecast = MonthlyDataByPersonnel.Where(x => x.Period > currentDate).Sum(x => x.GrossProfitDollarsForecast);

            return ytdGPD + currentMonthRemaingForecast + futureMonthsForecast;

        }


        #endregion






    }











    public class FiscalQuarter
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DaysInQuarter
        {
            get
            {
                return (int)((EndDate - StartDate).TotalDays);
            }
        }


        public static FiscalQuarter GetCurrent(DateTime currentDate, FiscalYear currentFiscalYear)
        {

            throw new NotImplementedException();

        }

        public static List<FiscalQuarter> GetQuarters(FiscalYear fiscalYear)
        {
            throw new NotImplementedException();
        }
    }

}
