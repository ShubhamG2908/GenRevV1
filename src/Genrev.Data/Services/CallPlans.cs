using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

using Genrev.Domain.DataSets;

namespace Genrev.Data.Services
{
    class CallPlans
    {

        public static List<CallPlanPerYearByAccountType> GetCallplanPerYearByAccountTypeByPersonnel(GenrevContext context, int personnelID, int year, DateTime currentDate) {

            var pPersonnelID = new SqlParameter("@PersonnelID", personnelID);
            var pYear = new SqlParameter("@Year", year);
            var pCurrentDate = new SqlParameter("@CurrentDate", currentDate);

            var data = context.Database.SqlQuery<CallPlanPerYearByAccountType>("dbo.GetCallPlanPerYearBySalesperson @PersonnelID, @Year, @CurrentDate",
                pPersonnelID, pYear, pCurrentDate);

            return data.ToList();

        }

        public static CallPlanOverviewDataset GetCallPlanOverviewByPersonnel(GenrevContext context, int fiscalYear, int personnelID)
        {

            var personnel = Genrev.Data.Services.General.GetDownstreamPersonnel(context, personnelID).ToList();

            IEnumerable<DTOs.CallsAnnualOverview> data = new List<DTOs.CallsAnnualOverview>();
            CallPlanOverviewDataset retVal = new CallPlanOverviewDataset(fiscalYear, personnel);


            SqlParameter pFiscalYear = new SqlParameter("@FiscalYear", fiscalYear);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnel.Select(x => x.ID).ToArray());
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.CallsAnnualOverview>
                ("ds.GetCallOverviewBySalespersons @FiscalYear, @PersonnelIDs",
                pFiscalYear, pPersonnelIDs).ToList();

            foreach(var item in data)
            {
                retVal.Add(
                    item.TypeName,
                    item.CallPlan,
                    item.CallGoal,
                    item.NumberOfAccounts,
                    item.SalesForecast);

            }

            return retVal;

        }



    }
}
