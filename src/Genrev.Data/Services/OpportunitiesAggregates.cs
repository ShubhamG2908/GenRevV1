using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.Services
{
    class OpportunitiesAggregates
    {

        public static List<Domain.DataSets.OpportunitiesAggregate> GetByPersonnel(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs) {

            IEnumerable<DTOs.OpportunitiesAggregate> data = new List<DTOs.OpportunitiesAggregate>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.OpportunitiesAggregate>("ds.GetOpportunitiesBySalespersons @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs);

            return getResults(data);

        }

        public static List<Domain.DataSets.OpportunitiesAggregate> GetByIndustry(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs) {

            IEnumerable<DTOs.OpportunitiesAggregate> data = new List<DTOs.OpportunitiesAggregate>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.OpportunitiesAggregate>("ds.GetOpportunitiesByIndustry @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs);

            return getResults(data);

        }

        public static List<Domain.DataSets.OpportunitiesAggregate> GetByCustomerType(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs) {

            IEnumerable<DTOs.OpportunitiesAggregate> data = new List<DTOs.OpportunitiesAggregate>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.OpportunitiesAggregate>("ds.GetOpportunitiesByCustomerType @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs);

            return getResults(data);

        }

        public static List<Domain.DataSets.OpportunitiesAggregate> GetByAccountType(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs) {

            IEnumerable<DTOs.OpportunitiesAggregate> data = new List<DTOs.OpportunitiesAggregate>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.OpportunitiesAggregate>("ds.GetOpportunitiesByAccountType @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs);

            return getResults(data);

        }

        public static List<Domain.DataSets.OpportunitiesAggregate> GetByCustomer(GenrevContext context, DateTime startDate, DateTime endDate, int[] personnelIDs)
        {

            IEnumerable<DTOs.OpportunitiesAggregate> data = new List<DTOs.OpportunitiesAggregate>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(personnelIDs);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<DTOs.OpportunitiesAggregate>("ds.GetOpportunitiesByCustomer @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs);

            return getResults(data);

        }


        private static List<Domain.DataSets.OpportunitiesAggregate> getResults(IEnumerable<DTOs.OpportunitiesAggregate> data) {

            var results = new List<Domain.DataSets.OpportunitiesAggregate>();

            foreach (var d in data) {
                var item = new Domain.DataSets.OpportunitiesAggregate();
                item.GroupEntityID = d.GroupEntityID;
                item.GroupEntityName = d.GroupEntityName;
                item.Potential = d.Potential;
                item.CurrentOpportunity = d.CurrentOpportunity;
                item.FutureOpportunity = d.FutureOpportunity;
                results.Add(item);
            }

            return results;

        }


    }
}
