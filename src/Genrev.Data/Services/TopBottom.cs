using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Domain.DataSets;
using System.Data.SqlClient;
using System.Data;

namespace Genrev.Data.Services
{
    class TopBottom
    {

        public static List<TopBottomMatrix> GetTopBottomMatrix(GenrevContext context, DateTime startDate, DateTime endDate, int[] pids) {

            IEnumerable<TopBottomMatrix> data = new List<TopBottomMatrix>();

            SqlParameter pStartDate = new SqlParameter("@StartDate", startDate);
            SqlParameter pEndDate = new SqlParameter("@EndDate", endDate);

            SqlParameter pPersonnelIDs = new SqlParameter("@PersonnelIDs", SqlDbType.Structured);
            pPersonnelIDs.Value = DTOs.Types.IDTableHelper.FromIntArray(pids);
            pPersonnelIDs.TypeName = "dbo.IDTable";

            data = context.Database.SqlQuery<TopBottomMatrix>(
                "ds.GetTopBottomMatrix @StartDate, @EndDate, @PersonnelIDs",
                pStartDate, pEndDate, pPersonnelIDs
                );

            return data.ToList();
        }

    }
}
