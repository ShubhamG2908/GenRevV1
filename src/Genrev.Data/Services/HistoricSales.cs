using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

namespace Genrev.Data.Services
{
    class HistoricSales
    {

        public static List<Domain.DataSets.HistoricSales> GetHistoricSales(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs, int[] accountTypeIDs, int[] productIDs, int[] customerIDs) {

            IEnumerable<DTOs.HistoricSales> data = new List<DTOs.HistoricSales>();

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

            SqlParameter pProductIDs = new SqlParameter("@ProductIDs", SqlDbType.Structured);
            pProductIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(productIDs);
            pProductIDs.TypeName = "dbo.IDTable";

            SqlParameter pCustomerIDs = new SqlParameter("@CustomerIDs", SqlDbType.Structured);
            pCustomerIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(customerIDs);
            pCustomerIDs.TypeName = "dbo.IDTable";


            data = context.Database.SqlQuery<DTOs.HistoricSales>(
                "ds.HistoricSales @StartDate, @EndDate, @PersonnelIDs, @IndustryIDs, @CustomerTypeIDs, @AccountTypeIDs, @ProductIDs, @CustomerIDs",
                pStartDate,
                pEndDate,
                pPersonnelIDs,
                pIndustryIDs,
                pCustomerTypeIDs,
                pAccountTypeIDs,
                pProductIDs,
                pCustomerIDs
            );

            var actualData = data.ToList();

            var results = new List<Domain.DataSets.HistoricSales>();

            foreach (var d in actualData) {

                var item = new Domain.DataSets.HistoricSales();

                item.CallsActual = d.CallsActual;
                item.CallsForecast = d.CallsForecast;
                item.CostActual = d.CostActual;
                item.CostForecast = d.CostForecast;
                item.FiscalYear = d.FiscalYear;
                item.SalesActual = d.SalesActual;
                item.SalesForecast = d.SalesForecast;
                item.Potential = d.Potential;

                results.Add(item);
            }

            return results;

        }

    }
}
