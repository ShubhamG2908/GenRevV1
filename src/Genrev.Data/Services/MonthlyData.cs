using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using Genrev.Domain.DataSets;

namespace Genrev.Data.Services
{
    class MonthyData
    {

        [Obsolete("Use Genrev.Data.Services.GetMonthlyData(context, startDate, endDate, filterParams) instead.")]
        public static List<Domain.DataSets.MonthlyData> GetMonthlyDataByPersonnel(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs, int[] accountTypeIDs) {

            IEnumerable<DTOs.MonthlyData> data = new List<DTOs.MonthlyData>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            SqlParameter pIndustryIDs = new SqlParameter("@IndustryIDs", SqlDbType.Structured);
            pIndustryIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(industryIDs);
            pIndustryIDs.TypeName = "dbo.IDTable";

            SqlParameter pCustomerTypeIDs = new SqlParameter("@CustomerTypeIDs", SqlDbType.Structured);
            pCustomerTypeIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(customerTypeIDs);
            pCustomerTypeIDs.TypeName = "dbo.IDTable";

            SqlParameter pAccountTypeIDs = new SqlParameter("@AccountTypeIDs", SqlDbType.Structured);
            pAccountTypeIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(accountTypeIDs);
            pAccountTypeIDs.TypeName = "dbo.IDTable";


            data = context.Database.SqlQuery<DTOs.MonthlyData>(
                "ds.DataByMonth @StartDate, @EndDate, @PersonnelIDs, @IndustryIDs, @CustomerTypeIDs",
                pStartDate,
                pEndDate,
                pPersonnelIDs,
                pIndustryIDs,
                pCustomerTypeIDs,
                pAccountTypeIDs
            );
            

            var results = new List<Domain.DataSets.MonthlyData>();

            foreach (var d in data) {

                var item = new Domain.DataSets.MonthlyData();

                item.CallsActual = d.CallsActual;
                item.CallsForecast = d.CallsForecast;
                item.CallsTarget = d.CallsTarget;
                item.CostActual = d.CostActual;
                item.CostForecast = d.CostForecast;
                item.CostTarget = d.CostTarget;
                item.Period = d.Period;
                item.SalesActual = d.SalesActual;
                item.SalesForecast = d.SalesForecast;
                item.SalesTarget = d.SalesTarget;
                item.Potential = d.Potential;

                results.Add(item);
            }

            return results;

        }

        internal static List<MonthlyData> GetMonthlyData(GenrevContext context, DateTime startDate, DateTime endDate, FilterParams filterParams) {


            IEnumerable<DTOs.MonthlyData> data = new List<DTOs.MonthlyData>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.PersonnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            SqlParameter pIndustryIDs = new SqlParameter("@IndustryIDs", SqlDbType.Structured);
            pIndustryIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.IndustryIDs);
            pIndustryIDs.TypeName = "dbo.IDTable";

            SqlParameter pCustomerTypeIDs = new SqlParameter("@CustomerTypeIDs", SqlDbType.Structured);
            pCustomerTypeIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.CustomerTypeIDs);
            pCustomerTypeIDs.TypeName = "dbo.IDTable";

            SqlParameter pAccountTypeIDs = new SqlParameter("@AccountTypeIDs", SqlDbType.Structured);
            pAccountTypeIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.AccountTypeIDs);
            pAccountTypeIDs.TypeName = "dbo.IDTable";

            SqlParameter pProductIDs = new SqlParameter("@ProductIDs", SqlDbType.Structured);
            pProductIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.ProductIDs);
            pProductIDs.TypeName = "dbo.IDTable";

            SqlParameter pCustomerIDs = new SqlParameter("@CustomerIDs", SqlDbType.Structured);
            pCustomerIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(filterParams.CustomerIDs);
            pCustomerIDs.TypeName = "dbo.IDTable";

            context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

            data = context.Database.SqlQuery<DTOs.MonthlyData>(
                "ds.DataByMonth @StartDate, @EndDate, @PersonnelIDs, @IndustryIDs, @CustomerTypeIDs, @AccountTypeIDs, @ProductIDs, @CustomerIDs",
                pStartDate,
                pEndDate,
                pPersonnelIDs,
                pIndustryIDs,
                pCustomerTypeIDs,
                pAccountTypeIDs,
                pProductIDs,
                pCustomerIDs
            ).ToList();
            
            var results = new List<MonthlyData>();
            
            foreach (var d in data) {

                var item = new MonthlyData();

                item.CallsActual = d.CallsActual;
                item.CallsForecast = d.CallsForecast;
                item.CallsTarget = d.CallsTarget;
                item.CostActual = d.CostActual;
                item.CostForecast = d.CostForecast;
                item.CostTarget = d.CostTarget;
                item.Period = d.Period;
                item.SalesActual = d.SalesActual;
                item.SalesForecast = d.SalesForecast;
                item.SalesTarget = d.SalesTarget;

                results.Add(item);
            }

            return results;


        }
    }
}
