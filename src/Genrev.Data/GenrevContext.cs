using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

using Genrev.Domain.Accounts;
using Genrev.Domain.Companies;
using Genrev.Domain.Users;
using Genrev.Domain.Products;
using Genrev.Domain.DataSets;
using Genrev.Domain.Data.Staging;

using System.Data;
using System.Data.SqlClient;
using Genrev.Domain.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Genrev.Data
{


    

    public class GenrevContext : DbContext
    {

        #region SETUP/CTORS

        const string WEB_SCHEMA = "aspnet";
        const string STAGING_SCHEMA = "staging";

        public GenrevContext() : 
            base(nameof(GenrevContext)) { construct(); }
        public GenrevContext(string connection) : base(connection) { construct(); }

        void construct() {
            Database.SetInitializer<GenrevContext>(null);
        }



        #endregion

        #region DB SETS

        public DbSet<Account> Accounts { get; set; }
        public DbSet<ActivityItem> AccountActivity { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Person> Personnel { get; set; }
        public DbSet<PersonnelAvailability> PersonnelAvailability { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<WebLoginHistory> WebLoginHistory { get; set; }
        public DbSet<WebMembership> WebMemberships { get; set; }
        public DbSet<WebPermissionGroup> WebPermissionGroups { get; set; }
        public DbSet<WebRole> WebRoles { get; set; }
        public DbSet<WebUserOption> WebUserOptions { get; set; }

        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<CustomerData> CustomerData { get; set; }
        public DbSet<CustomerDrilldown> CustomerDrilldowns { get; set; }
        public DbSet<ForecastLock> ForecastLocks { get; set; }

		public DbSet<Feedback> Feedback { get; set; }

        // API
        public DbSet<ApiToken> ApiTokens { get; set; }
        public DbSet<ApiIpWhitelistItem> ApiWhitelistItems { get; set; }




        // STAGING
        public DbSet<CompanyStaging> StagedCompanies { get; set; }
        public DbSet<PersonnelStaging> StagedPersonnel { get; set; }
        
        public DbSet<AccountTypeStaging> StagedAccountTypes { get; set; }
        public DbSet<CustomerTypeStaging> StagedCustomerTypes { get; set; }
        public DbSet<IndustryTypeStaging> StagedIndustryTypes { get; set; }

        public DbSet<CustomerStaging> StagedCustomers { get; set; }
        public DbSet<MonthlyDataStaging> StagedMonthlyData { get; set; }
        

        #endregion
        
        #region QUERIES



        public List<TopBottomMatrix> GetTopBottomMatrix(DateTime startDate, DateTime endDate, int[] personnel) {
            return Services.TopBottom.GetTopBottomMatrix(this, startDate, endDate, personnel);
        }
        
        public IEnumerable<Person> GetDownstreamPersonnel(int personID) {
            return Services.General.GetDownstreamPersonnel(this, personID);
        }
        public IEnumerable<Person> GetParentPersonnel(int personID) {
            return Services.General.GetParentPersonnel(this, personID);
        }
        public int[] GetDownstreamCustomerIDs(int personID) {
            return Services.General.GetDownstreamCustomerIDs(this, personID);
        }
        public int[] GetDownstreamPersonnelIDs(int personID) {
            return Services.General.GetDownstreamPersonnelIDs(this, personID);
        }
        


        public List<MonthlyData> GetMonthlyData(DateTime startDate, DateTime endDate, FilterParams filterParams) {
            return Services.MonthyData.GetMonthlyData(this, startDate, endDate, filterParams);
        }

        public List<OpportunitiesAggregate> GetOpportunitiesAggregateByPersonnel(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.OpportunitiesAggregates.GetByPersonnel(this, startDate, endDate, personnelIDs);
        }

        public List<OpportunitiesAggregate> GetOpportunitiesAggregateByIndustry(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.OpportunitiesAggregates.GetByIndustry(this, startDate, endDate, personnelIDs);
        }

        public List<OpportunitiesAggregate> GetOpportunitiesAggregateByCustomerType(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.OpportunitiesAggregates.GetByCustomerType(this, startDate, endDate, personnelIDs);
        }

        public List<OpportunitiesAggregate> GetOpportunitiesAggregateByAccountType(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.OpportunitiesAggregates.GetByAccountType(this, startDate, endDate, personnelIDs);
        }

        public List<OpportunitiesAggregate> GetOpportunitiesAggregateByCustomer(DateTime startDate, DateTime endDate, int[] personnelIDs)
        {
            return Services.OpportunitiesAggregates.GetByCustomer(this, startDate, endDate, personnelIDs);
        }




        [Obsolete("Use GenrevContext.GetMonthlyData(startDate, endDate, filterParams) instead")]
        public List<MonthlyData> GetMonthlyData(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.MonthyData.GetMonthlyDataByPersonnel(this, startDate, endDate, personnelIDs, null, null, null);
        }

        [Obsolete("Use GenrevContext.GetMonthlyData(startDate, endDate, filterParams) instead")]
        public List<MonthlyData> GetMonthlyData(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs) {
            return Services.MonthyData.GetMonthlyDataByPersonnel(this, startDate, endDate, personnelIDs, industryIDs, null, null);
        }

        [Obsolete("Use GenrevContext.GetMonthlyData(startDate, endDate, filterParams) instead")]
        public List<MonthlyData> GetMonthlyData(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs) {
            return Services.MonthyData.GetMonthlyDataByPersonnel(this, startDate, endDate, personnelIDs, industryIDs, customerTypeIDs, null);
        }


        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs) {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, null, null, null, null, null);
        }

        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs) {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, industryIDs, null, null, null, null);
        }

        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs) {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, industryIDs, customerTypeIDs, null, null, null);
        }

        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs, int[] accountTypeIDs) {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, industryIDs, customerTypeIDs, accountTypeIDs, null, null);
        }

        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs, int[] accountTypeIDs, int[] productIDs) {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, industryIDs, customerTypeIDs, accountTypeIDs, productIDs, null);
        }

        public List<HistoricSales> GetHistoricSales(DateTime startDate, DateTime endDate, int[] personnelIDs, int[] industryIDs, int[] customerTypeIDs, int[] accountTypeIDs, int[] productIDs, int[] customerIDs)
        {
            return Services.HistoricSales.GetHistoricSales(this, startDate, endDate, personnelIDs, industryIDs, customerTypeIDs, accountTypeIDs, productIDs, customerIDs);
        }

        public List<CallPlanPerYearByAccountType> GetCallPlanPerYearByAccountTypeByPersonnel(int personnelID, int year, DateTime currentDate) {
            return Services.CallPlans.GetCallplanPerYearByAccountTypeByPersonnel(this, personnelID, year, currentDate);
        }

        public CallPlanOverviewDataset GetCallPlanOverviewByPersonnel(int fiscalYear, int personnelID) {
            return Services.CallPlans.GetCallPlanOverviewByPersonnel(this, fiscalYear, personnelID);
        }

        #endregion

        #region COMMANDS

        // STAGING UPSERTS
        public void UpsertAccountTypesStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertAccountTypesToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertCompaniesStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertCompaniesToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertCustomerStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertCustomersToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertCustomerTypesStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertCustomerTypesToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertIndustryTypesStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertIndustryTypesToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertMonthlyDataStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertMonthlyDataToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }

        public void UpsertPersonnelStagingToLive(int accountID) {
            Database.ExecuteSqlCommand("EXEC staging.UpsertPersonnelToLive @AccountID", new SqlParameter("@AccountID", accountID));
        }


        // ACCOUNT PROVISIONING

        public DTOs.AccountProvisionResults ProvisionAccount(
            string email, string companyFullName, string companyName, string companyCode,
            int fiscalYearMonthEnd, string loginFirstName, string loginLastName, string loginDisplayName, 
            string loginGender) {


            //EXEC dbo.ProvisionAccount
            //    @AccountEmail = 'jleach@dymeng.com',
            //    @CompanyFullName = 'Dymeng Services, Inc.',
            //    @CompanyName = 'Dymeng',
            //    @CompanyCode = 'DYM',
            //    @CompanyFiscalMonthEnd = 12,
            //    @SysAdminFirstName = 'Jack',
            //    @SysAdminLastName = 'Leach',
            //    @SysAdminGender = 'M';

            try {

                var results = Database.SqlQuery<DTOs.AccountProvisionResults>(
                "ProvisionAccount " +
                "    @AccountEmail, @CompanyFullName, @CompanyName, @CompanyCode, @CompanyFiscalMonthEnd, " +
                "    @SysAdminFirstName, @SysAdminLastName, @SysAdminGender",
                new SqlParameter() { ParameterName = "@AccountEmail", SqlDbType = SqlDbType.VarChar, SqlValue = email },
                new SqlParameter() { ParameterName = "@CompanyFullName", SqlDbType = SqlDbType.VarChar, SqlValue = companyFullName },
                new SqlParameter() { ParameterName = "@CompanyName", SqlDbType = SqlDbType.VarChar, SqlValue = companyName },
                new SqlParameter() { ParameterName = "@CompanyCode", SqlDbType = SqlDbType.VarChar, SqlValue = companyCode },
                new SqlParameter() { ParameterName = "@CompanyFiscalMonthEnd", SqlDbType = SqlDbType.Int, SqlValue = fiscalYearMonthEnd },
                new SqlParameter() { ParameterName = "@SysAdminFirstName", SqlDbType = SqlDbType.VarChar, SqlValue = loginFirstName },
                new SqlParameter() { ParameterName = "@SysAdminLastName", SqlDbType = SqlDbType.VarChar, SqlValue = loginLastName },
                new SqlParameter() { ParameterName = "@SysAdminGender", SqlDbType = SqlDbType.VarChar, SqlValue = loginGender }
                );

                return results.First();

            } catch (Exception e) {

                System.Diagnostics.Debug.WriteLine("EXCEPTION=======================");
                System.Diagnostics.Debug.WriteLine(e.ToString());

                return null;

            }
            

        }


        #endregion

        #region MODEL BUILDING

        private void buildUserModels(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<Role>().Property(e => e.IsSysAdministrator).HasColumnName("RoleIsSysAdministrator");
            modelBuilder.Entity<Role>().Property(e => e.IsSysSalesPro).HasColumnName("RoleIsSysSalesPro");
            modelBuilder.Entity<Role>().Property(e => e.Name).HasColumnName("RoleName");
            modelBuilder.Entity<Role>().Property(e => e.Code).HasColumnName("RoleCode");
            modelBuilder.Entity<Role>().Property(e => e.Description).HasColumnName("RoleDescription");
            modelBuilder.Entity<Role>()
                .HasMany(p => p.Personnel)
                .WithMany(r => r.Roles)
                .Map(r => {
                    r.ToTable("PersonnelRoles");
                    r.MapLeftKey("RoleID");
                    r.MapRightKey("PersonnelID");
                });

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<User>().Property(e => e.DisplayName).HasColumnName("UserDisplayName");
            modelBuilder.Entity<User>().Property(e => e.Email).HasColumnName("UserEmail");
            modelBuilder.Entity<User>().HasKey(x => x.ID);
            modelBuilder.Entity<User>()
                .HasOptional(x => x.MembershipDetail)
                .WithRequired(x => x.User);
            modelBuilder.Entity<User>()
                .HasMany(r => r.Roles)
                .WithMany(u => u.Users)
                .Map(r => {
                    r.ToTable("UserRoles", WEB_SCHEMA);
                    r.MapLeftKey("UserID");
                    r.MapRightKey("RoleName");
                });

            modelBuilder.Entity<WebLoginHistory>().ToTable("UserLoginHistory", WEB_SCHEMA);
            modelBuilder.Entity<WebLoginHistory>().Property(e => e.Activity).HasColumnName("LoginHistoryActivity");
            modelBuilder.Entity<WebLoginHistory>().Property(e => e.DateOfAction).HasColumnName("LoginHistoryDateOfActionUTC");
            modelBuilder.Entity<WebLoginHistory>().Property(e => e.Note).HasColumnName("LoginHistoryNote");

            modelBuilder.Entity<WebMembership>().ToTable("UserMembership", WEB_SCHEMA);
            modelBuilder.Entity<WebMembership>().Property(e => e.Password).HasColumnName("MemberPassword");
            modelBuilder.Entity<WebMembership>().Property(e => e.PasswordQuestion).HasColumnName("MemberPasswordQuestion");
            modelBuilder.Entity<WebMembership>().Property(e => e.PasswordAnswer).HasColumnName("MemberPasswordAnswer");
            modelBuilder.Entity<WebMembership>().Property(e => e.IsApproved).HasColumnName("MemberIsApproved");
            modelBuilder.Entity<WebMembership>().Property(e => e.LastActivityDate).HasColumnName("MemberLastActivityDateUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.LastLoginDate).HasColumnName("MemberLastLoginDateUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.LastPasswordChangeDate).HasColumnName("MemberLastPasswordChangedDateUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.CreationDate).HasColumnName("MemberCreationDateUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.IsLockedOut).HasColumnName("MemberIsLockedOut");
            modelBuilder.Entity<WebMembership>().Property(e => e.LastLockoutDate).HasColumnName("MemberLastLockoutDateUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.FailedPasswordAttempts).HasColumnName("MemberFailedPasswordAttemptCount");
            modelBuilder.Entity<WebMembership>().Property(e => e.FailedPasswordWindowStart).HasColumnName("MemberFailedPasswordWindowStartUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.FailedPasswordAnswerAttemptCount).HasColumnName("MemberFailedPasswordAnswerAttemptCount");
            modelBuilder.Entity<WebMembership>().Property(e => e.FailedPasswordAnswerAttemptWindowStart).HasColumnName("MemberFailedPasswordAnswerAttemptWindowStartUTC");
            modelBuilder.Entity<WebMembership>().Property(e => e.ID).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<WebPermissionGroup>().ToTable("PermissionGroups", WEB_SCHEMA);
            modelBuilder.Entity<WebPermissionGroup>().HasKey(x => x.Name);
            modelBuilder.Entity<WebPermissionGroup>().Property(x => x.Name).HasColumnName("GroupName");
            modelBuilder.Entity<WebPermissionGroup>().Property(x => x.DisplayName).HasColumnName("GroupDisplayName");
            modelBuilder.Entity<WebPermissionGroup>().Property(x => x.Description).HasColumnName("GroupDescription");

            modelBuilder.Entity<WebRole>().ToTable("Roles", WEB_SCHEMA);
            modelBuilder.Entity<WebRole>().HasKey(x => x.Name);
            modelBuilder.Entity<WebRole>().Property(x => x.Name).HasColumnName("RoleName");
            modelBuilder.Entity<WebRole>().Property(x => x.GroupName).HasColumnName("PermissionGroupName");
            modelBuilder.Entity<WebRole>().Property(x => x.DisplayName).HasColumnName("RoleDisplayName");
            modelBuilder.Entity<WebRole>().Property(x => x.Description).HasColumnName("RoleDescription");
            modelBuilder.Entity<WebRole>()
                .HasRequired(x => x.PermissionGroup)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.GroupName);
            modelBuilder.Entity<WebRole>()
                .HasMany(x => x.Users)
                .WithMany(x => x.Roles)
                .Map(r => {
                    r.ToTable("UserRoles", WEB_SCHEMA);
                    r.MapLeftKey("RoleName");
                    r.MapRightKey("UserID");
                });

            modelBuilder.Entity<WebUserOption>().ToTable("UserOptions", WEB_SCHEMA);
            modelBuilder.Entity<WebUserOption>().Property(e => e.OptionName).HasColumnName("OptionName");
            modelBuilder.Entity<WebUserOption>().Property(e => e.ValueRaw).HasColumnName("OptionValue");
        }

        private void buildDomainModels(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<Account>().Property(e => e.Status).HasColumnName("AccountStatus");
            modelBuilder.Entity<Account>().Property(e => e.Email).HasColumnName("AccountEmail");
            modelBuilder.Entity<Account>().Property(e => e.ApiEnabled).HasColumnName("AccountApiEnabled");
            modelBuilder.Entity<Account>().Property(e => e.ApiKey).HasColumnName("AccountApiKey");
            modelBuilder.Entity<Account>().Property(e => e.AllowApiIpBypass).HasColumnName("AccountApiAllowIpBypass");
            modelBuilder.Entity<Account>().Property(e => e.ApiPassword).HasColumnName("AccountApiPassword");
            
            modelBuilder.Entity<ActivityItem>().ToTable("AccountActivity");
            modelBuilder.Entity<ActivityItem>().Property(e => e.Type).HasColumnName("ActivityCode");
            modelBuilder.Entity<ActivityItem>().Property(e => e.DateOfActivity).HasColumnName("ActivityDate");
            modelBuilder.Entity<ActivityItem>().Property(e => e.Note).HasColumnName("ActivityNote");

            modelBuilder.Entity<Company>().Property(e => e.FullName).HasColumnName("CompanyFullName");
            modelBuilder.Entity<Company>().Property(e => e.Name).HasColumnName("CompanyName");
            modelBuilder.Entity<Company>().Property(e => e.Code).HasColumnName("CompanyCode");
            modelBuilder.Entity<Company>().Property(e => e.FiscalYearEndMonth).HasColumnName("CompanyFiscalMonthEnd");

            modelBuilder.Entity<Customer>().ToTable("CompanyCustomers");
            modelBuilder.Entity<Customer>().Property(e => e.Name).HasColumnName("CustomerName");
            modelBuilder.Entity<Customer>().Property(e => e.Address1).HasColumnName("CustomerAddress1");
            modelBuilder.Entity<Customer>().Property(e => e.Address2).HasColumnName("CustomerAddress2");
            modelBuilder.Entity<Customer>().Property(e => e.City).HasColumnName("CustomerCity");
            modelBuilder.Entity<Customer>().Property(e => e.State).HasColumnName("CustomerState");
            modelBuilder.Entity<Customer>().Property(e => e.Country).HasColumnName("CustomerCountry");
            modelBuilder.Entity<Customer>().Property(e => e.PostalCode).HasColumnName("CustomerPostalCode");
            modelBuilder.Entity<Customer>().Property(e => e.Phone).HasColumnName("CustomerPhone");
            modelBuilder.Entity<Customer>().Property(e => e.TypeID).HasColumnName("CustomerTypeID");
            modelBuilder.Entity<Customer>().Property(e => e.IndustryID).HasColumnName("CustomerIndustryID");
            modelBuilder.Entity<Customer>().Property(e => e.AccountTypeID).HasColumnName("CustomerAccountTypeID");
            modelBuilder.Entity<Customer>()
                .HasMany(p => p.Personnel)
                .WithMany(c => c.Customers)
                .Map(r => {
                    r.ToTable("CompanyCustomersPersonnel");
                    r.MapLeftKey("CustomerID");
                    r.MapRightKey("PersonnelID");
                });
            modelBuilder.Entity<Customer>()
                .HasMany(p => p.Products)
                .WithMany(c => c.Customers)
                .Map(r => {
                    r.ToTable("CompanyCustomerProducts");
                    r.MapLeftKey("CustomerID");
                    r.MapRightKey("ProductID");
                });

            modelBuilder.Entity<Feedback>().ToTable("Feedback");

            modelBuilder.Entity<CustomerDrilldown>().ToTable("CustomerDrilldown");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.AccountTypeName).HasColumnName("AccountType");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CustomerTypeName).HasColumnName("CustomerType");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.IndustryName).HasColumnName("Industry");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CallsActual).HasColumnName("DataCallsActual");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CallsForecast).HasColumnName("DataCallsForecast");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CallsTarget).HasColumnName("DataCallsTarget");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.Potential).HasColumnName("DataPotential");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CurrentOpportunity).HasColumnName("DataCurrentOpportunity");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.FutureOpportunity).HasColumnName("DataFutureOpportunity");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.SalesActual).HasColumnName("DataSalesActual");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.SalesForecast).HasColumnName("DataSalesForecast");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.SalesTarget).HasColumnName("DataSalesTarget");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CostActual).HasColumnName("DataCostActual");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CostForecast).HasColumnName("DataCostForecast");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.CostTarget).HasColumnName("DataCostTarget");
            modelBuilder.Entity<CustomerDrilldown>().Property(x => x.Period).HasColumnName("DataPeriod");
            modelBuilder.Entity<CustomerDrilldown>()
                .HasRequired(x => x.Person)
                .WithMany(x => x.CustomerDrilldowns)
                .HasForeignKey(x => x.PersonnelID);

            modelBuilder.Entity<CustomerType>().ToTable("CompanyCustomerTypes");
            modelBuilder.Entity<CustomerType>().Property(e => e.Name).HasColumnName("TypeName");

            modelBuilder.Entity<Industry>().ToTable("CompanyIndustries");
            modelBuilder.Entity<Industry>().Property(e => e.Name).HasColumnName("IndustryName");

            modelBuilder.Entity<AccountType>().ToTable("CompanyAccountTypes");
            modelBuilder.Entity<AccountType>().Property(e => e.Name).HasColumnName("AccountTypeName");
            modelBuilder.Entity<AccountType>().Property(e => e.CallsPerMonthGoal).HasColumnName("AccountTypeCallsPerMonthGoal");

            modelBuilder.Entity<Person>().ToTable("Personnel");
            modelBuilder.Entity<Person>().HasKey(x => x.ID);
            modelBuilder.Entity<Person>().Property(e => e.FirstName).HasColumnName("PersonFirstName");
            modelBuilder.Entity<Person>().Property(e => e.LastName).HasColumnName("PersonLastName");
            modelBuilder.Entity<Person>().Property(e => e.Gender).HasColumnName("PersonGender");
            modelBuilder.Entity<Person>()
                .HasMany(c => c.Customers)
                .WithMany(p => p.Personnel)
                .Map(r => {
                    r.ToTable("CompanyCustomersPersonnel");
                    r.MapLeftKey("PersonnelID");
                    r.MapRightKey("CustomerID");
                });

            modelBuilder.Entity<PersonnelAvailability>().ToTable("PersonnelAvailability");
            modelBuilder.Entity<PersonnelAvailability>().Property(x => x.AdministrationDays).HasColumnName("AvailableAdministrationDays");
            modelBuilder.Entity<PersonnelAvailability>().Property(x => x.Holidays).HasColumnName("AvailableHolidayDays");
            modelBuilder.Entity<PersonnelAvailability>().Property(x => x.OtherDays).HasColumnName("AvailableOtherDays");
            modelBuilder.Entity<PersonnelAvailability>().Property(x => x.VacationDays).HasColumnName("AvailableVacationDays");
            modelBuilder.Entity<PersonnelAvailability>().Property(x => x.Weekdays).HasColumnName("AvailableWeekdays");
            modelBuilder.Entity<PersonnelAvailability>()
                .HasRequired(x => x.Person)
                .WithMany(x => x.Availability)
                .HasForeignKey(x => x.PersonnelID);


            modelBuilder.Entity<ProductGroup>().ToTable("CompanyProductGroups");
            modelBuilder.Entity<ProductGroup>().Property(e => e.Name).HasColumnName("GroupName");

            modelBuilder.Entity<Product>().ToTable("CompanyProducts");
            modelBuilder.Entity<Product>().Property(e => e.Description).HasColumnName("ProductDescription");
            modelBuilder.Entity<Product>().Property(e => e.SKU).HasColumnName("ProductSKU");
            modelBuilder.Entity<Product>()
                .HasMany(c => c.Customers)
                .WithMany(p => p.Products)
                .Map(r => {
                    r.ToTable("CompanyCustomerProducts");
                    r.MapLeftKey("ProductID");
                    r.MapRightKey("CustomerID");
                });

            modelBuilder.Entity<CustomerData>().ToTable("CustomerData");
            modelBuilder.Entity<CustomerData>().Property(e => e.Period).HasColumnName("DataPeriod");
            modelBuilder.Entity<CustomerData>().Property(e => e.SalesActual).HasColumnName("DataSalesActual");
            modelBuilder.Entity<CustomerData>().Property(e => e.SalesForecast).HasColumnName("DataSalesForecast");
            modelBuilder.Entity<CustomerData>().Property(e => e.SalesTarget).HasColumnName("DataSalesTarget");
            modelBuilder.Entity<CustomerData>().Property(e => e.Potential).HasColumnName("DataPotential");
            modelBuilder.Entity<CustomerData>().Property(e => e.CurrentOpportunity).HasColumnName("DataCurrentOpportunity");
            modelBuilder.Entity<CustomerData>().Property(e => e.FutureOpportunity).HasColumnName("DataFutureOpportunity");
            modelBuilder.Entity<CustomerData>().Property(e => e.CostActual).HasColumnName("DataCostActual");
            modelBuilder.Entity<CustomerData>().Property(e => e.CostForecast).HasColumnName("DataCostForecast");
            modelBuilder.Entity<CustomerData>().Property(e => e.CostTarget).HasColumnName("DataCostTarget");
            modelBuilder.Entity<CustomerData>().Property(e => e.CallsActual).HasColumnName("DataCallsActual");
            modelBuilder.Entity<CustomerData>().Property(e => e.CallsForecast).HasColumnName("DataCallsForecast");
            modelBuilder.Entity<CustomerData>().Property(e => e.CallsTarget).HasColumnName("DataCallsTarget");
            modelBuilder.Entity<CustomerData>()
                .HasOptional(x => x.Person)
                .WithMany(x => x.CustomerData)
                .HasForeignKey(x => x.PersonnelID);
            modelBuilder.Entity<CustomerData>()
                .HasOptional(x => x.Product)
                .WithMany(x => x.CustomerData)
                .HasForeignKey(x => x.ProductID);

            var forecastLockMapping = modelBuilder.Entity<ForecastLock>();
            forecastLockMapping.HasKey(m => m.ID);
            forecastLockMapping.Property(m => m.ID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            forecastLockMapping.Property(m => m.PersonnelID).IsRequired().HasColumnAnnotation("Index",new IndexAnnotation(new IndexAttribute("UQ_Personnel_Year", 0) { IsUnique = true }));
            forecastLockMapping.Property(m => m.Year).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("UQ_Personnel_Year", 1) { IsUnique = true }));

            forecastLockMapping.HasRequired(m => m.Person)
                .WithMany()
                .HasForeignKey(m => m.PersonnelID);
        }

        private void buildStagingModels(DbModelBuilder modelBuilder) {


            modelBuilder.Entity<CompanyStaging>().ToTable("Companies", STAGING_SCHEMA);
            modelBuilder.Entity<CompanyStaging>().Property(x => x.ClientID).HasColumnName("CompanyClientID");
            modelBuilder.Entity<CompanyStaging>().Property(x => x.Code).HasColumnName("CompanyCode");
            modelBuilder.Entity<CompanyStaging>().Property(x => x.FiscalMonthEnd).HasColumnName("CompanyFiscalMonthEnd");
            modelBuilder.Entity<CompanyStaging>().Property(x => x.FullName).HasColumnName("CompanyFullName");
            modelBuilder.Entity<CompanyStaging>().Property(x => x.Name).HasColumnName("CompanyName");
            modelBuilder.Entity<CompanyStaging>().Property(x => x.ParentClientID).HasColumnName("CompanyParentClientID");

            modelBuilder.Entity<PersonnelStaging>().ToTable("Personnel", STAGING_SCHEMA);
            modelBuilder.Entity<PersonnelStaging>().Property(x => x.ClientID).HasColumnName("PersonClientID");
            modelBuilder.Entity<PersonnelStaging>().Property(x => x.FirstName).HasColumnName("PersonFirstName");
            modelBuilder.Entity<PersonnelStaging>().Property(x => x.LastName).HasColumnName("PersonLastName");

            modelBuilder.Entity<AccountTypeStaging>().ToTable("AccountTypes", STAGING_SCHEMA);
            modelBuilder.Entity<AccountTypeStaging>().Property(x => x.CallsPerMonthGoal).HasColumnName("TypeCallsPerMonthGoal");
            modelBuilder.Entity<AccountTypeStaging>().Property(x => x.ClientID).HasColumnName("TypeClientID");
            modelBuilder.Entity<AccountTypeStaging>().Property(x => x.Name).HasColumnName("TypeName");

            modelBuilder.Entity<CustomerTypeStaging>().ToTable("CustomerTypes", STAGING_SCHEMA);
            modelBuilder.Entity<CustomerTypeStaging>().Property(x => x.ClientID).HasColumnName("TypeClientID");
            modelBuilder.Entity<CustomerTypeStaging>().Property(x => x.Name).HasColumnName("TypeName");

            modelBuilder.Entity<IndustryTypeStaging>().ToTable("IndustryTypes", STAGING_SCHEMA);
            modelBuilder.Entity<IndustryTypeStaging>().Property(x => x.ClientID).HasColumnName("TypeClientID");
            modelBuilder.Entity<IndustryTypeStaging>().Property(x => x.Name).HasColumnName("TypeName");

            modelBuilder.Entity<CustomerStaging>().ToTable("Customers", STAGING_SCHEMA);
            modelBuilder.Entity<CustomerStaging>().Property(x => x.ClientID).HasColumnName("CustomerClientID");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.Name).HasColumnName("CustomerName");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.CustomerTypeClientID).HasColumnName("CustomerTypeClientID");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.AccountTypeClientID).HasColumnName("CustomerAccountTypeClientID");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.IndustryTypeClientID).HasColumnName("CustomerIndustryTypeClientID");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.Address1).HasColumnName("CustomerAddress1");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.Address2).HasColumnName("CustomerAddress2");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.City).HasColumnName("CustomerCity");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.State).HasColumnName("CustomerState");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.Country).HasColumnName("CustomerCountry");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.Phone).HasColumnName("CustomerPhone");
            modelBuilder.Entity<CustomerStaging>().Property(x => x.PostalCode).HasColumnName("CustomerPostalCode");

            modelBuilder.Entity<MonthlyDataStaging>().ToTable("CustomerData", STAGING_SCHEMA);
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CustomerClientID).HasColumnName("DataCustomerClientID");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.PersonClientID).HasColumnName("DataPersonClientID");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.ProductClientID).HasColumnName("DataProductClientID");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.SalesActual).HasColumnName("DataSalesActual");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.SalesTarget).HasColumnName("DataSalesTarget");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CostActual).HasColumnName("DataCostActual");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CostTarget).HasColumnName("DataCostTarget");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CallsActual).HasColumnName("DataCallsActual");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CallsTarget).HasColumnName("DataCallsTarget");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.Potential).HasColumnName("DataPotential");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.CurrentOpportunity).HasColumnName("DataCurrentOpportunity");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.FutureOpportunity).HasColumnName("DataFutureOpportunity");
            modelBuilder.Entity<MonthlyDataStaging>().Property(x => x.Period).HasColumnName("DataPeriod");



        }

        private void buildApiModels(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<ApiToken>().ToTable("AccountApiTokens");
            modelBuilder.Entity<ApiToken>().Property(x => x.ExpirationDate).HasColumnName("TokenExpirationDate");
            modelBuilder.Entity<ApiToken>().Property(x => x.GenerationDate).HasColumnName("TokenGenerationDate");
            modelBuilder.Entity<ApiToken>().Property(x => x.Value).HasColumnName("TokenValue");
            modelBuilder.Entity<ApiToken>()
                .HasRequired(x => x.Account)
                .WithMany(x => x.ApiTokens)
                .HasForeignKey(x => x.AccountID);


            modelBuilder.Entity<ApiIpWhitelistItem>().ToTable("AccountApiIpWhitelist");
            modelBuilder.Entity<ApiIpWhitelistItem>().Property(x => x.RangeEnd).HasColumnName("IpRangeEnd");
            modelBuilder.Entity<ApiIpWhitelistItem>().Property(x => x.RangeStart).HasColumnName("IpRangeStart");
            modelBuilder.Entity<ApiIpWhitelistItem>()
                .HasRequired(x => x.Account)
                .WithMany(x => x.ApiIpWhitelistItems)
                .HasForeignKey(x => x.AccountID);




        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            buildUserModels(modelBuilder);

            buildDomainModels(modelBuilder);

            buildStagingModels(modelBuilder);

            buildApiModels(modelBuilder);
            
        }

        #endregion

    }
}


// http://stackoverflow.com/questions/11191103/entity-framework-read-only-collections