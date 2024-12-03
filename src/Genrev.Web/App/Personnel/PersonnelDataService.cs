using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Web.App.Personnel.Models;
using System.Data;
using System.Data.SqlClient;

namespace Genrev.Web.App.Personnel
{
    public class PersonnelDataService
    {



        public void ToggleReportsTo(int sourcePersonID, int targetPersonID) {

            var sql = "SELECT TOP 1 ParentPersonnelID FROM dbo.PersonnelHierarchy WHERE PersonnelID = @Source AND ParentPersonnelID = @Target;";

            var result = dataContext.Database.SqlQuery<int?>(
                sql,
                new[] {
                    new SqlParameter() { ParameterName = "@Source", SqlDbType = SqlDbType.Int, SqlValue = sourcePersonID },
                    new SqlParameter() { ParameterName = "@Target", SqlDbType = SqlDbType.Int, SqlValue = targetPersonID }
                });

            if (result.Count() == 0) {
                // toggle on
                dataContext.Database.ExecuteSqlCommand(
                    "INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@Source, @Target)",
                    new[] {
                        new SqlParameter() { ParameterName = "@Source", SqlDbType = SqlDbType.Int, SqlValue = sourcePersonID },
                        new SqlParameter() { ParameterName = "@Target", SqlDbType = SqlDbType.Int, SqlValue = targetPersonID }
                    });
            } else {
                // toggle off
                dataContext.Database.ExecuteSqlCommand(
                    "DELETE FROM dbo.PersonnelHierarchy WHERE PersonnelID = @Source AND ParentPersonnelID = @Target;",
                    new[] {
                        new SqlParameter() { ParameterName = "@Source", SqlDbType = SqlDbType.Int, SqlValue = sourcePersonID },
                        new SqlParameter() { ParameterName = "@Target", SqlDbType = SqlDbType.Int, SqlValue = targetPersonID }
                    });
            }

        }

        internal List<AvailabilityListItem> GetAvailabilityListItems(int personnelID) {

            var items = new List<AvailabilityListItem>();
            var data = dataContext.PersonnelAvailability.Where(x => x.PersonnelID == personnelID).OrderBy(x => x.AvailabilityYear);

            foreach (var d in data) {

                var item = new AvailabilityListItem();

                item.AdministrationDays = d.AdministrationDays;
                item.Holidays = d.Holidays;
                item.ID = d.ID;
                item.OtherDays = d.OtherDays;
                item.PersonnelID = d.PersonnelID;
                item.PlannedCalls = d.PlannedCallsPerDay;
                item.VacationDays = d.VacationDays;
                item.Weekdays = d.Weekdays;
                item.Year = d.AvailabilityYear;

                item.CallCommittment = d.CallCommitment;
                item.DaysAvailable = d.DaysAvailable;

                items.Add(item);
            }

            return items;
        }


        /// <summary>
        /// Get a list of all personnel under this account's primary company, marked with which are ReportsTo of the specified personnel
        /// </summary>
        /// <param name="basePersonnelID"></param>
        /// <returns></returns>
        public List<ReportsToListItemVM> GetReportsToListItems(Domain.Accounts.Account account, int basePersonnelID) {

            List<Domain.Companies.Person> parentPersonnel = dataContext.GetParentPersonnel(basePersonnelID).ToList();

            List<Domain.Companies.Person> allPersonnel = account.PrimaryCompany.Personnel
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var items = new List<ReportsToListItemVM>();

            foreach (var p in allPersonnel) {

                if (p.ID != basePersonnelID) {

                    var item = new ReportsToListItemVM();
                    item.ID = p.ID;
                    item.FirstName = p.FirstName;
                    item.LastName = p.LastName;

                    if (parentPersonnel.Where(x => x.ID == p.ID).FirstOrDefault() != null) {
                        item.ReportsTo = true;
                    }

                    items.Add(item);
                }
            }

            return items;

        }

        internal void UpdateAvailabilityItems(List<AvailabilityListItem> updates) {

            var items = dataContext.PersonnelAvailability;

            foreach (var update in updates) {

                var item = items.Find(update.ID);

                item.AdministrationDays = update.AdministrationDays;
                item.AvailabilityYear = update.Year;
                item.Holidays = update.Holidays;
                item.OtherDays = update.OtherDays;
                item.PlannedCallsPerDay = update.PlannedCalls;
                item.VacationDays = update.VacationDays;
                item.Weekdays = update.Weekdays;
                
            }
            
            dataContext.SaveChanges();
        }

        internal void InsertAvailabilityItems(List<AvailabilityListItem> inserts, int personnelID) {

            var items = dataContext.PersonnelAvailability;

            foreach (var insert in inserts) {

                var item = new Domain.Companies.PersonnelAvailability();
                item.PersonnelID = personnelID;
                item.AdministrationDays = insert.AdministrationDays;
                item.AvailabilityYear = insert.Year;
                item.Holidays = insert.Holidays;
                item.OtherDays = insert.OtherDays;
                item.PlannedCallsPerDay = insert.PlannedCalls;
                item.VacationDays = insert.VacationDays;
                item.Weekdays = insert.Weekdays;

                items.Add(item);
            }

            dataContext.SaveChanges();
        }

        internal void DeleteAvailabilityItems(List<int> deleteKeys) {
            
            foreach (int id in deleteKeys) {
                var item = dataContext.PersonnelAvailability.Find(id);
                dataContext.PersonnelAvailability.Remove(item);
            }

            dataContext.SaveChanges();

        }


        /// <summary>
        /// Get a list of all personnel under this account's primary company
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<OverviewListItemVM> GetOverviewListItems(Domain.Accounts.Account account) {
            
            var personnel = account.PrimaryCompany.Personnel
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var items = new List<OverviewListItemVM>();

            foreach (var p in personnel) {
                items.Add(new OverviewListItemVM()
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsAdmin = p.Roles.Any(x => x.IsSysAdministrator)
                });
            }

            return items;
        }
        
        public void ToggleAdmin(int personnelID)
        {
            var person = dataContext.Personnel.Where(x => x.ID == personnelID).Single();

            if(person.Roles.Any(x => x.IsSysAdministrator))
            {
                foreach (var r in person.Roles.Where(x => x.IsSysAdministrator))
                {
                    person.RemoveRole(r);
                }
            }
            else
            {
                var adminRole = dataContext.Roles.Where(x => x.Name == "sysadmin").First();
                person.AddRole(adminRole);
            }

            dataContext.SaveChanges();

        }






        private Genrev.Data.GenrevContext dataContext;

        public PersonnelDataService() {
            dataContext = new Genrev.Data.GenrevContext();
        }

        public PersonnelDataService(Genrev.Data.GenrevContext context) {
            dataContext = context;
        }


    }
}