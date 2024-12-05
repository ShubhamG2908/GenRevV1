using System;

namespace Genrev.Domain.DataSets
{
    public class MonthlyData : DataAggregateBase
    {
        public DateTime Period { get; set; }

        public decimal? MonthToDateSalesForecast(DateTime currentDate)
        {
            DateTime currentMonthStart = new DateTime(currentDate.Year, currentDate.Month, 1);

            // if we're prior to the current month return null
            if (currentMonthStart < Period)
            {
                return null;
            }

            // if we're after the current month return null
            if (currentMonthStart >= Period.AddMonths(1))
            {
                return null;
            }

            // if we're on the current month, calculate the MTD
            int daysInMonth = currentMonthStart.AddMonths(1).AddDays(-1).Day;

            // Current day should be the day before the current day according to formula.
            //int currentDay = currentDate.Day;     // Old
            int currentDay = currentDate.Day - 1;   // New

            if (currentDay == daysInMonth) {
                return SalesForecast;
            }

            if (currentDay > daysInMonth)
            {
                throw new InvalidOperationException("Current Day exceeds Days in Month");
            }

            //decimal percentOfMonthCompleted = 100 * currentDay / daysInMonth;
            //decimal? mtdForecast = percentOfMonthCompleted * SalesForecast / 100;

            // Apply new formula
            decimal? mtdForecast = (SalesForecast * currentDay) / daysInMonth;

            return mtdForecast;
        }


        public decimal? MonthToDateCallsForecast(DateTime currentDate)
        {

            DateTime currentMonthStart = new DateTime(currentDate.Year, currentDate.Month, 1);

            // if we're prior to the current month return null
            if (currentMonthStart < Period)
            {
                return null;
            }

            // if we're after the current month return null
            if (currentMonthStart >= Period.AddMonths(1))
            {
                return null;
            }

            // if we're on the current month, calculate the MTD
            int daysInMonth = currentMonthStart.AddMonths(1).AddDays(-1).Day;
            int currentDay = currentDate.Day;

            if (currentDay == daysInMonth)
            {
                return CallsForecast;
            }

            if (currentDay > daysInMonth)
            {
                throw new InvalidOperationException("Current Day exceeds Days in Month");
            }

            decimal percentOfMonthCompleted = 100 * currentDay / daysInMonth;

            decimal? mtdForecast = percentOfMonthCompleted * CallsForecast / 100;

            return mtdForecast;
        }

        public class Projections
        {
            public static decimal? MonthEndGPD(decimal? currentMTDGPD, DateTime currentDate)
            {
                return MonthEndSales(currentMTDGPD, currentDate);
            }

            public static decimal? MonthEndSales(decimal? currentMTDSales, DateTime currentDate)
            {
                int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
                var x = (currentMTDSales / (currentDate.Day <= 1 ? 1 : currentDate.Day - 1));
                var y = x * daysInMonth;
                return General.FixDollars(y);
            }

            public static decimal? YearEndSales(decimal? currentYTDSales, DateTime currentDate, Month fiscalYearMonthEnd)
            {

                FiscalYear fy = FiscalYear.GetCurrent(currentDate, fiscalYearMonthEnd);

                // OLD Code 
                //var x = currentYTDSales * fy.DaysInYear;
                //var y = x / currentDate.DayOfYear;

                //return General.FixDollars(y);

                // Duplicate code so calling directly that method.
                return YearEndSales(currentYTDSales, currentDate, fy);
            }

            public static decimal? MonthEndGPP(decimal? sales, decimal? grossProfitDollars, DateTime currentDate)
            {
                var salesProjection = MonthEndSales(sales, currentDate);
                var gpdProjection = MonthEndGPD(grossProfitDollars, currentDate);

                if (salesProjection == 0)
                {
                    return 0;
                }
                return (((salesProjection - gpdProjection) / salesProjection) - 1) * -1 * 100;
            }

            public static decimal? YearEndGPP(decimal? currentYTDSales, decimal? currentYTDGPD, DateTime currentDate, FiscalYear fy)
            {

                var salesProjection = YearEndSales(currentYTDSales, currentDate, fy);
                var gpdProjection = YearEndGPD(currentYTDGPD, currentDate, fy);

                if (salesProjection == 0)
                {
                    return 0;
                }

                return (((salesProjection - gpdProjection) / salesProjection) - 1) * -1 * 100;
            }

            public static decimal? YearEndSales(decimal? currentYTDSales, DateTime currentDate, FiscalYear fiscalYear) {

                // Old Code
                //var x = currentYTDSales * fiscalYear.DaysInYear;
                //var y = x / currentDate.DayOfYear;

                // New changed code according to the new equations.
                int dayOfYear = currentDate.DayOfYear - 1;
                var x = currentYTDSales * (fiscalYear.DaysInYear + 1);
                var y = x / (dayOfYear < 1 ? 1 : dayOfYear);

                return General.FixDollars(y);
            }

            public static decimal? YearEndGPD(decimal? currentYTDGPD, DateTime currentDate, FiscalYear fy)
            {
                return YearEndSales(currentYTDGPD, currentDate, fy);
            }
        }
    }
}
