using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using Genrev.Domain.Accounts;
using Genrev.Domain.Companies;
using Genrev.Domain.Users;

namespace Genrev.Data.Services
{
    class General
    {

        public static IEnumerable<Person> GetDownstreamPersonnel(GenrevContext context, int personID) {
            var personnel = context.Database.SqlQuery<Person>(
                "GetDownstreamPersonnel @PersonID",
                new SqlParameter() { ParameterName = "@PersonID", SqlDbType = SqlDbType.Int, SqlValue = personID }).ToList();

            foreach(var p in personnel)
            {
                p.Availability = context.PersonnelAvailability.Where(x => x.PersonnelID == p.ID).ToList();
            }

            return personnel;
        }

        public static IEnumerable<Person> GetParentPersonnel(GenrevContext context, int personID) {
            
            var sql = "SELECT p.* " +
                "FROM dbo.Personnel AS p " +
                "WHERE ID IN( " +
                "  SELECT ParentPersonnelID FROM dbo.PersonnelHierarchy WHERE PersonnelID = @ID " +
                ");";

            var results = context.Database.SqlQuery<DTOs.ParentPersonnelResults>(
                    sql,
                    new SqlParameter() { ParameterName = "@ID", SqlDbType = System.Data.SqlDbType.Int, SqlValue = personID }
                );

            var items = new List<Person>();

            foreach (var r in results) {

                items.Add(new Person()
                {
                    ID = r.ID,
                    CompanyID = r.CompanyID,
                    DateCreated = r.DateCreated,
                    FirstName = r.PersonFirstName,
                    Gender = r.PersonGender,
                    LastName = r.PersonLastName,
                });
            }

            return items;

        }

        internal static int[] GetDownstreamCustomerIDs(GenrevContext context, int personID) {

            var ret = context.Database.SqlQuery<int>(
                "GetDownstreamCustomerIDs @PersonID",
                new SqlParameter() { ParameterName = "@PersonID", SqlDbType = SqlDbType.Int, SqlValue = personID });

            return ret.ToArray();

        }

        internal static int[] GetDownstreamPersonnelIDs(GenrevContext context, int personID) {

            var ret = context.Database.SqlQuery<int>(
                "GetDownstreamPersonnelIDs @PersonID",
                new SqlParameter() { ParameterName = "@PersonID", SqlDbType = SqlDbType.Int, SqlValue = personID });

            return ret.ToArray();
        }
    }
}
