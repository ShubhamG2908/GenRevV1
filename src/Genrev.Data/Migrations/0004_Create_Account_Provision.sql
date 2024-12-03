

 -- /*
CREATE PROCEDURE [dbo].[ProvisionAccount](
	@AccountEmail NVARCHAR(128),
	@CompanyFullName NVARCHAR(256),
	@CompanyName NVARCHAR(128),
	@CompanyCode NVARCHAR(12),
	@CompanyFiscalMonthEnd INT,
	@SysAdminFirstName NVARCHAR(50),
	@SysAdminLastName NVARCHAR(50),
	@SysAdminGender NVARCHAR(1)
) AS 
-- */

	 /* TEST DATA
	DECLARE @AccountEmail NVARCHAR(128)
	DECLARE @CompanyFullName NVARCHAR(256)
	DECLARE @CompanyName NVARCHAR(128)
	DECLARE @CompanyCode NVARCHAR(12)
	DECLARE @CompanyFiscalMonthEnd INT
	DECLARE @SysAdminFirstName NVARCHAR(50)
	DECLARE @SysAdminLastName NVARCHAR(50)
	DECLARE @SysAdminGender NVARCHAR(1)
	SET @AccountEmail = 'info@dymeng.com'
	SET @CompanyFullName = 'Dymeng Services, Inc.'
	SET @CompanyName = 'Dymeng'
	SET @CompanyCode = 'DYM'
	SET @CompanyFiscalMonthEnd = 12
	SET @SysAdminFirstName = 'Jack'
	SET @SysAdminLastName = 'Leach'
	SET @SysAdminGender = 'M'
	-- */
	
	DECLARE @AccountID INT
	DECLARE @CompanyID INT
	DECLARE @PersonnelID INT
	DECLARE @SARoleID INT

	-- Create temp output table
	IF OBJECT_ID('tempdb..#ProvisionResults') IS NOT NULL DROP TABLE #ProvisionResults
	CREATE TABLE #ProvisionResults (
		AccountID INT, CompanyID INT, PersonnelID INT
	);

	
	BEGIN TRY

		BEGIN TRANSACTION PROVISIONACCOUNT

			INSERT INTO dbo.Accounts (AccountStatus, AccountEmail) VALUES (1, @AccountEmail);
			SET @AccountID = SCOPE_IDENTITY()

			INSERT INTO dbo.AccountActivity (AccountID, ActivityCode, ActivityDate, ActivityNote)
				VALUES (@AccountID, 0, GETDATE(), 'System Created Account');

			INSERT INTO dbo.Companies (AccountID, CompanyFullName, CompanyName, CompanyCode, CompanyFiscalMonthEnd) 
				VALUES (@AccountID, @CompanyFullName, @CompanyName, @CompanyCode, @CompanyFiscalMonthEnd);
			SET @CompanyID = SCOPE_IDENTITY()

			INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName, PersonGender) 
				VALUES (@CompanyID, @SysAdminFirstName, @SysAdminLastName, @SysAdminGender);
			SET @PersonnelID = SCOPE_IDENTITY()

			INSERT INTO dbo.Roles (CompanyID, RoleIsSysAdministrator, RoleName, RoleCode, RoleDescription)
				VALUES (@CompanyID, 1, 'sysadmin', 'sa', 'System Administrator');
			SET @SARoleID = SCOPE_IDENTITY()

			INSERT INTO dbo.Roles (CompanyID, RoleIsSysSalesPro, RoleName, RoleCode, RoleDescription)
				VALUES (@CompanyID, 1, 'salespro', 'sp', 'Sales Professional');

			INSERT INTO dbo.PersonnelRoles (PersonnelID, RoleID) VALUES (@PersonnelID, @SARoleID);

		COMMIT TRANSACTION PROVISIONACCOUNT

		INSERT INTO #ProvisionResults (AccountID, CompanyID, PersonnelID) VALUES (@AccountID, @CompanyID, @PersonnelID);

		SELECT * FROM #ProvisionResults;

		RETURN

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION PROVISIONACCOUNT
		;THROW
	END CATCH


GO



