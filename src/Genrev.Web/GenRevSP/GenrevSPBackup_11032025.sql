
CREATE OR ALTER FUNCTION [dbo].[FullMonthsSeparation] 
(
    @DateA DATETIME,
    @DateB DATETIME
)
RETURNS INT
AS
BEGIN
    DECLARE @Result INT

    DECLARE @DateX DATETIME
    DECLARE @DateY DATETIME

    IF(@DateA < @DateB)
    BEGIN
    	SET @DateX = @DateA
    	SET @DateY = @DateB
    END
    ELSE
    BEGIN
    	SET @DateX = @DateB
    	SET @DateY = @DateA
    END

    SET @Result = (
    				SELECT 
    				CASE 
    					WHEN DATEPART(DAY, @DateX) > DATEPART(DAY, @DateY)
    					THEN DATEDIFF(MONTH, @DateX, @DateY) - 1
    					ELSE DATEDIFF(MONTH, @DateX, @DateY)
    				END
    				)

    RETURN @Result
END
GO
/****** Object:  UserDefinedFunction [dbo].[GetFiscalYearMonths]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- ==============================
-- Table function for returning months of a fiscal year
-- ==============================
CREATE OR ALTER FUNCTION [dbo].[GetFiscalYearMonths] (	
	@CompanyID int,
	@Year int
) RETURNS @months TABLE (
	[Month] date
) AS BEGIN
	DECLARE @FiscalMonthEnd int
	DECLARE @End date
	DECLARE @Start date

	SELECT @FiscalMonthEnd = CO.CompanyFiscalMonthEnd
	FROM dbo.Companies AS CO
	WHERE CO.ID = @CompanyID

	SET @End = DATEFROMPARTS(@Year,@FiscalMonthEnd,1)
	SET @Start = DATEADD(YEAR,-1,DATEADD(MONTH,1, @End))

	INSERT @months
	SELECT  
		CONVERT(date, DATEADD(MONTH, x.number - 1, @Start),111) AS [Month]
	FROM    dbo.Numbers x
	WHERE   x.number <= DATEDIFF(MONTH, @Start, @End) + 1
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[CreateUser]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER   PROCEDURE [dbo].[CreateUser]
    @Email NVARCHAR(128),
    @Password NVARCHAR(500),
    @SecurityQuestion NVARCHAR(500),
    @SecurityAnswer NVARCHAR(128),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Gender NVARCHAR(1),
    @CompanyName NVARCHAR(256),
    @CompanyCode NVARCHAR(12),
    @FiscalMonthEnd INT,
    @CountryCode NVARCHAR(5)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Step 1: Create Account
        DECLARE @AccountID INT;
        IF NOT EXISTS (SELECT 1 FROM dbo.Accounts WHERE AccountEmail = @Email)
        BEGIN
            INSERT INTO dbo.Accounts (
                DateCreated, AccountStatus, AccountEmail
            )
            VALUES (
                GETDATE(), 1, @Email
            );
            SET @AccountID = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            SELECT @AccountID = ID FROM dbo.Accounts WHERE AccountEmail = @Email;
        END

        -- Step 2: Create the Company
        DECLARE @CompanyID INT;
        IF NOT EXISTS (
            SELECT 1 FROM dbo.Companies
            WHERE AccountID = @AccountID AND CompanyCode = @CompanyCode
        )
        BEGIN
            INSERT INTO dbo.Companies (
                DateCreated, AccountID, CompanyFullName, CompanyName,
                CompanyCode, CompanyFiscalMonthEnd, CountryCode
            )
            VALUES (
                GETDATE(), @AccountID, @CompanyName, @CompanyName,
                @CompanyCode, @FiscalMonthEnd, @CountryCode
            );
            SET @CompanyID = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            SELECT @CompanyID = ID FROM dbo.Companies
            WHERE AccountID = @AccountID AND CompanyCode = @CompanyCode;
        END

        -- Step 3: Create the Personnel
        INSERT INTO dbo.Personnel (
            DateCreated, CompanyID, PersonFirstName, PersonLastName, PersonGender
        )
        VALUES (
            GETDATE(), @CompanyID, @FirstName, @LastName, @Gender
        );
        DECLARE @PersonnelID INT = SCOPE_IDENTITY();

        -- Step 4: Create the User
        INSERT INTO dbo.Users (
            DateCreated,
            AccountID,
            UserEmail,
            UserDisplayName,
            PersonnelID
        )
        VALUES (
            GETDATE(),
            @AccountID,
            @Email,
            @FirstName + ' ' + @LastName,
            @PersonnelID
        );
        DECLARE @UserMembershipID INT = SCOPE_IDENTITY();

        -- Step 5: Add the User in the UserMembership table
        INSERT INTO aspnet.UserMembership (
            ID,
            DateCreated,
            MemberPassword,
            MemberPasswordQuestion,
            MemberPasswordAnswer,
            MemberLastActivityDateUTC,
            MemberIsApproved,
            MemberCreationDateUTC,
            MemberLastLockoutDateUTC,
            MemberFailedPasswordAttemptCount,
            MemberFailedPasswordAnswerAttemptCount,
            MemberFailedPasswordAnswerAttemptWindowStartUTC
        )
        VALUES (
            @UserMembershipID,
            GETDATE(),
            @Password,
            @SecurityQuestion,
            @SecurityAnswer,
            GETDATE(),
            1,
            GETDATE(),
            '1900-01-01',
            0,
            0,
            '1900-01-01'
        );

        -- Setp 6: Add New Role For Company
        INSERT INTO Roles (CompanyID, RoleIsSysAdministrator, RoleIsSysSalesPro, RoleCode, RoleName, RoleDescription)
        VALUES (@CompanyID, 1,0,'sa','sysadmin','System Administrator')
        DECLARE @AdminRoleId INT = SCOPE_IDENTITY();

        INSERT INTO Roles (CompanyID, RoleIsSysAdministrator, RoleIsSysSalesPro, RoleCode, RoleName, RoleDescription)
        VALUES (@CompanyID, 0,1,'sp','salespro','Sales Professional')

        -- Setp 7: Assing Admin Role To Person
        INSERT INTO PersonnelRoles (DateCreated, PersonnelID, RoleID)
        VALUES (GETDATE(), @PersonnelID, @AdminRoleId)

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrState INT = ERROR_STATE();
        RAISERROR(@ErrMsg, @ErrSeverity, @ErrState);
    END CATCH
END;
GO
/****** Object:  StoredProcedure [dbo].[GetCallPlanPerYearBySalesperson]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[GetCallPlanPerYearBySalesperson] (@PersonnelID INT, @Year INT, @CurrentDate DATE) AS
	
	DECLARE @CompanyID INT
	
	SET @CompanyID = (
		SELECT TOP 1 c.ID
		FROM dbo.Companies AS c
		INNER JOIN dbo.Personnel AS p ON p.ID = @PersonnelID
	)

	SELECT 
		@PersonnelID AS PersonnelID,
		base.AccountTypeID,
		base.AccountTypeName,
		CAST(ISNULL(SUM(base.CalcCallsBase),0) as float) AS PlannedCalls,
		CAST(ISNULL(base.GoalCountPerYear,0) as float) as GoalCountPerYear,
		COUNT(DISTINCT base.CustomerAccountTypeCountString) AS AccountTypeCount

	FROM (

		SELECT 
			acct.ID AS AccountTypeID,
			acct.AccountTypeName,
			CAST(ISNULL(acct.AccountTypeCallsPerMonthGoal * 12,0) as float) AS GoalCountPerYear,
			CONVERT(VARCHAR(128), cust.ID) + '|' + CONVERT(VARCHAR(128), cust.CustomerAccountTypeID) AS CustomerAccountTypeCountString,
			CASE WHEN COALESCE(dat.DataPeriod, GETDATE()) >= @CurrentDate THEN COALESCE(dat.DataCallsActual, 0) ELSE COALESCE(dat.DataCallsForecast, 0) END AS CalcCallsBase
	
		FROM dbo.CompanyAccountTypes AS acct

		LEFT JOIN (

			SELECT cc.*
			FROM dbo.CompanyCustomers AS cc
			INNER JOIN dbo.CompanyCustomersPersonnel AS ccp ON cc.ID = ccp.CustomerID
			WHERE ccp.PersonnelID = @PersonnelID

		) AS cust  ON cust.CustomerAccountTypeID = acct.ID

		LEFT JOIN dbo.CustomerDataWithFiscal AS dat ON dat.CustomerID = cust.ID

		WHERE acct.CompanyID = @CompanyID
			AND dat.FiscalYear = @Year

	) AS base

	GROUP BY
		base.AccountTypeID,
		base.AccountTypeName,
		base.GoalCountPerYear

	ORDER BY base.AccountTypeName
;
GO
/****** Object:  StoredProcedure [dbo].[GetDownstreamCustomerIDs]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








 CREATE OR ALTER PROCEDURE [dbo].[GetDownstreamCustomerIDs] (@PersonID INT) AS BEGIN

	 /* TEST DATA
		DECLARE @PersonID INT
		SET @PersonID = 39
	-- */

	IF OBJECT_ID('tempdb..#DCIDPersonnel') IS NOT NULL 
		DROP TABLE #DCIDPersonnel;

	CREATE  TABLE #DCIDPersonnel (
		ID INT
	);

	INSERT INTO #DCIDPersonnel EXECUTE dbo.GetDownstreamPersonnelIDs @PersonID = @PersonID;
	
	SELECT
		c.ID
	FROM dbo.CompanyCustomers AS c
	INNER JOIN dbo.CompanyCustomersPersonnel AS cp ON cp.CustomerID = c.ID
	INNER JOIN #DCIDPersonnel AS p ON p.ID = cp.PersonnelID
	
	RETURN

END

GO
/****** Object:  StoredProcedure [dbo].[GetDownstreamPersonnel]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[GetDownstreamPersonnel] (@PersonID INT) AS 
	
	/*
		One Person can report to any number of other persons.
		Circular dependencies are allowed.  A standard recursive CTE
		will fail due to the circular recursion, so include a 
		sentinal string that maintains a list of added IDs, and
		verify each entry before including it in the next recursed set.

		Append a delimeter to the end of each ID before putting it in
		the sentinal so as to avoid false positives on the sentinal
		check (e.g., exists 2134 and check for 13, so instead it becomes
		2134. and check for 13. using a dot delimiter)

		Return a table of distinct Persons for all persons downstream of 
		(and including) the specified @PersonID
	*/

	DECLARE @Results TABLE(ID INT, ParentPersonnel INT, Sentinal NVARCHAR(MAX))

	 /* Test Data
	DECLARE @PersonID INT
	SET @PersonID = 6
	-- */

	;WITH tree (PersonnelID, ParentPersonnelID, Sentinal) AS
	(
		SELECT ah.PersonnelID, ah.ParentPersonnelID, Sentinal = CAST(ah.PersonnelID AS VARCHAR(MAX)) + '.'
		FROM dbo.PersonnelHierarchy AS ah -- anchor hierarchy
		WHERE ah.ParentPersonnelID = @PersonID

		UNION ALL

		SELECT rh.PersonnelID, rh.ParentPersonnelID, Sentinal + '.' +  CAST(rh.PersonnelID AS VARCHAR(MAX)) + '.'
		FROM dbo.PersonnelHierarchy AS rh -- recursion hierarchy
		JOIN tree AS t ON t.PersonnelID = rh.ParentPersonnelID
			AND rh.PersonnelID <> rh.ParentPersonnelID
		WHERE CHARINDEX(CAST(rh.PersonnelID AS VARCHAR(MAX)) + '.', sentinal) = 0
	)
	
	INSERT INTO @Results SELECT * FROM tree OPTION(MAXRECURSION 500)
	-- SELECT * FROM @Results

	-- get a full table of Personnel joined to the distinct list of results
	SELECT 
		p.ID,
		p.DateCreated,
		p.rv,
		p.CompanyID,
		p.PersonFirstName AS FirstName,
		p.PersonLastName AS LastName,
		p.PersonGender AS Gender,
		p.ClientID 
	FROM dbo.Personnel AS p
	INNER JOIN (
		SELECT DISTINCT ID FROM @Results UNION SELECT @PersonID
	) AS h ON p.ID = h.ID

	RETURN


GO
/****** Object:  StoredProcedure [dbo].[GetDownstreamPersonnelIDs]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[GetDownstreamPersonnelIDs] (@PersonID INT) AS 
	
	/*
		One Person can report to any number of other persons.
		Circular dependencies are allowed.  A standard recursive CTE
		will fail due to the circular recursion, so include a 
		sentinal string that maintains a list of added IDs, and
		verify each entry before including it in the next recursed set.

		Append a delimeter to the end of each ID before putting it in
		the sentinal so as to avoid false positives on the sentinal
		check (e.g., exists 2134 and check for 13, so instead it becomes
		2134. and check for 13. using a dot delimiter)

		Return a table of distinct Persons for all persons downstream of 
		(and including) the specified @PersonID
	*/

	DECLARE @Results TABLE(ID INT, ParentPersonnel INT, Sentinal NVARCHAR(MAX))

	 /* Test Data
	DECLARE @PersonID INT
	SET @PersonID = 6
	-- */

	;WITH tree (PersonnelID, ParentPersonnelID, Sentinal) AS
	(
		SELECT ah.PersonnelID, ah.ParentPersonnelID, Sentinal = CAST(ah.PersonnelID AS VARCHAR(MAX)) + '.'
		FROM dbo.PersonnelHierarchy AS ah -- anchor hierarchy
		WHERE ah.ParentPersonnelID = @PersonID

		UNION ALL

		SELECT rh.PersonnelID, rh.ParentPersonnelID, Sentinal + '.' +  CAST(rh.PersonnelID AS VARCHAR(MAX)) + '.'
		FROM dbo.PersonnelHierarchy AS rh -- recursion hierarchy
		JOIN tree AS t ON t.PersonnelID = rh.ParentPersonnelID
			AND rh.PersonnelID <> rh.ParentPersonnelID
		WHERE CHARINDEX(CAST(rh.PersonnelID AS VARCHAR(MAX)) + '.', sentinal) = 0
	)
	
	INSERT INTO @Results SELECT * FROM tree OPTION(MAXRECURSION 500)
	-- SELECT * FROM @Results

	-- get a full table of Personnel joined to the distinct list of results
	SELECT 
		p.ID
	FROM dbo.Personnel AS p
	INNER JOIN (
		SELECT DISTINCT ID FROM @Results UNION SELECT @PersonID
	) AS h ON p.ID = h.ID

	RETURN



GO
/****** Object:  StoredProcedure [dbo].[GetForecastData]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- ==============================
-- Forecast data
-- ==============================
CREATE OR ALTER PROCEDURE [dbo].[GetForecastData]
	@FiscalYear int,
	@PersonnelID int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CompanyID int

	--SET @FiscalYear = 2016
	--SET @PersonnelID = 16

	SELECT @CompanyID = P.CompanyID
	FROM Personnel AS P
	WHERE P.ID = @PersonnelID

	SELECT
		COALESCE(DATA.DataPeriod, MC.[Month]) AS Period, 
		MC.CustomerID,
		MC.CustomerName, 
		DATA.ID AS CustomerDataID,
		DATA.DataSalesForecast AS SalesForecast, 
		DATA.DataSalesTarget AS SalesTarget, 
		DATA.DataCostForecast AS CostForecast, 
		DATA.DataCostTarget AS CostTarget, 
		DATA.DataCallsForecast AS CallsForecast, 
		DATA.DataCallsTarget AS CallsTarget, 
		DATA.DataPotential AS Potential,
		DATA.DataCurrentOpportunity AS CurrentOpportunity, 
		DATA.DataFutureOpportunity AS FutureOpportunity, 
		DATA.Strategy,
		DATA.MarketShare,
		DATA.AtRisk,
		DATA.RiskExplanation 
	FROM (
		SELECT 
			M.*,
			CCUST.ID AS CustomerID,
			CCUST.CustomerName,
			CCUST.PersonnelID
		FROM dbo.GetFiscalYearMonths(@CompanyID,@FiscalYear) AS M CROSS JOIN (
			SELECT 
				CUST.*,
				CCP.PersonnelID
			FROM CompanyCustomersPersonnel AS CCP INNER JOIN
				 CompanyCustomers AS CUST ON CCP.CustomerID = CUST.ID
		) AS CCUST
	) AS MC LEFT JOIN (
		SELECT
			CD.ID,
			CD.CustomerID,
			CD.PersonnelID,
			CD.DataPeriod, 
			CD.DataSalesForecast, 
			CD.DataSalesTarget, 
			CD.DataCostForecast, 
			CD.DataCostTarget, 
			CD.DataCallsForecast, 
			CD.DataCallsTarget, 
			CD.DataPotential,
			CD.DataCurrentOpportunity, 
			CD.DataFutureOpportunity, 
			CD.Strategy,
			CD.MarketShare,
			CD.AtRisk,
			CD.RiskExplanation
		FROM CustomerData AS CD 
	) AS DATA ON MC.[Month] = DATA.DataPeriod AND MC.PersonnelID = DATA.PersonnelID AND MC.CustomerID = DATA.CustomerID
	WHERE MC.PersonnelID = @PersonnelID
	ORDER BY MC.[Month], MC.CustomerName
END
GO
/****** Object:  StoredProcedure [dbo].[InitializeInstance]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- /*
CREATE OR ALTER PROCEDURE [dbo].[InitializeInstance] (
	@AccountEmail NVARCHAR(128),
	@CompanyFullName NVARCHAR(256),
	@CompanyName NVARCHAR(128),
	@CompanyCode NVARCHAR(12),
	@CompanyFiscalMonthEnd INT,
	@SysAdminFirstName NVARCHAR(50),
	@SysAdminLastName NVARCHAR(50),
	@SysAdminGender NVARCHAR(1),
	@PasswordHash NVARCHAR(500)
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
	DECLARE @PasswordHash NVARCHAR(500)
	SET @AccountEmail = 'info@dymeng.com'
	SET @CompanyFullName = 'Dymeng Services, Inc.'
	SET @CompanyName = 'Dymeng'
	SET @CompanyCode = 'DYM'
	SET @CompanyFiscalMonthEnd = 12
	SET @SysAdminFirstName = 'Jack'
	SET @SysAdminLastName = 'Leach'
	SET @SysAdminGender = 'M'
	SET @PasswordHash = '+Q6qHuX2S9kW2hjz0i4tKYlp7TQ='
	-- */
	
	DECLARE @AccountID INT
	DECLARE @CompanyID INT
	DECLARE @PersonnelID INT
		
	IF OBJECT_ID('tempdb..#ProvisionInfo') IS NOT NULL DROP TABLE #ProvisionInfo
	CREATE TABLE #ProvisionInfo (
		AccountID INT, CompanyID INT, PersonnelID INT
	);
	
	/*
		This exec call populates the following tables:
			dbo.Accounts
			dbo.AccountActivity
			dbo.Compaines
			dbo.Personnel
			dbo.Roles
			dbo.PersonnelRoles
	*/	
	INSERT INTO #ProvisionInfo
		EXEC dbo.ProvisionAccount
			@AccountEmail = @AccountEmail,
			@CompanyFullName = @CompanyFullName,
			@CompanyName = @CompanyName,
			@CompanyCode = @CompanyCode,
			@CompanyFiscalMonthEnd = @CompanyFiscalMonthEnd,
			@SysAdminFirstName  = @SysAdminFirstName,
			@SysAdminLastName = @SysAdminLastName,
			@SysAdminGender = @SysAdminGender;
			
	SELECT TOP 1
		@AccountID = t.AccountID,
		@CompanyID = t.CompanyID,
		@PersonnelID = t.PersonnelID
	FROM #ProvisionInfo AS t;
	
	/*
		We then have to populate these tables:
			dbo.Users
			aspnet.UserMembership
	*/
	
	INSERT INTO dbo.Users (AccountID, PersonnelID, UserEmail, UserDisplayName)
		VALUES (@AccountID, @PersonnelID, @AccountEmail, LOWER(LEFT(@SysAdminFirstName, 1) + @SysAdminLastName));
		
	DECLARE @UserID INT;
	SET @UserID = (SELECT TOP 1 ID FROM dbo.Users);
	
	INSERT INTO aspnet.UserMembership (ID, MemberPassword) VALUES (@UserID, @PasswordHash);
	
GO
/****** Object:  StoredProcedure [dbo].[ProvisionAccount]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


 -- /*
CREATE OR ALTER PROCEDURE [dbo].[ProvisionAccount](
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
/****** Object:  StoredProcedure [ds].[DataByMonth]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =========================================
-- Return monthly breakdown per the specified criteria
-- see in-prod notes for parameter details
-- =========================================
-- /* 
CREATE OR ALTER PROCEDURE [ds].[DataByMonth](
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY,
	@IndustryIDs IDTable READONLY,
	@CustomerTypeIDs IDTable READONLY,
	@AccountTypeIDs IDTable READONLY,
	@ProductIDs IDTable READONLY,
	@CustomerIDs IDTable READONLY
	) AS 
BEGIN
-- */
	

	/* 
		PersonnelID list is required
		Other ID lists (IndustryIDs, CustomerTypeIDs etc) are optional
		If records are found in non-personnel lists,
		  then will be filtered accordingly, otherwise all matching
		  rows are included.
	*/

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable
	DECLARE @IndustryIDs IDTable
	DECLARE @CustomerTypeIDs IDTable
	DECLARE @AccountTypeIDs IDTable
	DECLARE @ProductIDs IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)

	-- INSERT INTO @IndustryIDs (ID) VALUES (6)
	-- INSERT INTO @IndustryIDs (ID) VALUES (7)
	-- INSERT INTO @IndustryIDs (ID) VALUES (9)

	INSERT INTO @ProductIDs (ID) VALUES (2)
	

	-- INSERT INTO @CustomerTypeIDs (ID) VALUES (3)
	-- INSERT INTO @CustomerTypeIDs (ID) VALUES (4)

	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;


	DECLARE @NumMonths INT
	SET @NumMonths = dbo.FullMonthsSeparation(@StartDate, @EndDate) + 1;
	
	
	DECLARE @HasFilters BIT
	DECLARE @FilterOnIndustries BIT
	DECLARE @FilterOnCustomerTypes BIT
	DECLARE @FilterOnAccountTypes BIT
	DECLARE @FilterOnProducts BIT
	DECLARE @FilterOnCustomers BIT
	
	SET @FilterOnIndustries = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @IndustryIDs)
	SET @FilterOnCustomerTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerTypeIDs)
	SET @FilterOnAccountTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @AccountTypeIDs)
	SET @FilterOnProducts = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @ProductIDs)
	SET @FilterOnCustomers = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerIDs)
	
	DECLARE @Statement NVARCHAR(2000)

	SET @Statement = 
		'SELECT data.*, periods.* FROM ( 
			SELECT 
				DATENAME(month, cd.DataPeriod) as DataPeriod, 
				SUM(COALESCE(cd.DataSalesActual, 0)) AS SalesActual, 
				SUM(COALESCE(cd.DataSalesForecast, 0)) AS SalesForecast,
				SUM(COALESCE(cd.DataSalesTarget, 0)) AS SalesTarget, 
				SUM(COALESCE(cd.DataCostActual, 0)) AS CostActual, 
				SUM(COALESCE(cd.DataCostForecast, 0)) AS CostForecast,
				SUM(COALESCE(cd.DataCostTarget, 0)) AS CostTarget,
				SUM(COALESCE(cd.DataCallsActual, 0)) AS CallsActual, 
				SUM(COALESCE(cd.DataCallsForecast, 0)) AS CallsForecast,
				SUM(COALESCE(cd.DataCallsTarget, 0)) AS CallsTarget,
				SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
				SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
				SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity
			FROM dbo.CustomerData AS cd 
			INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 

			[ADDITIONAL_JOINS] 

			WHERE cd.DataPeriod >= @StartDate 
				AND cd.DataPeriod <= @EndDate 
			GROUP BY DATENAME(month, cd.DataPeriod)  
		
		) AS data 

		RIGHT JOIN ( 
		
			SELECT Number, DATEADD(MONTH, Number - 1, @StartDate) AS Period 
			FROM dbo.Numbers  
			WHERE Number <= @NumMonths 

		) AS periods ON DATENAME(month, periods.Period) = data.DataPeriod 

		ORDER BY periods.Period'

	IF @FilterOnIndustries = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @IndustryIDs AS iid ON c.CustomerIndustryID = iid.ID ')
	END;

	IF @FilterOnCustomerTypes = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerTypeIDs AS ctids ON c.CustomerTypeID = ctids.ID ')
	END;

	IF @FilterOnAccountTypes = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @AccountTypeIDs AS atids ON c.CustomerAccountTypeID = atids.ID ')
	END;

	IF @FilterOnProducts = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @ProductIDs AS prodIDs ON cd.ProductID = prodIDs.ID ')
	END;

	IF @FilterOnCustomers = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerIDs AS custIDs ON cd.CustomerID = custIDs.ID ')
	END;

	-- if we applied additional filters, those will need the company customers joined to resolve
	IF @HasFilters = 1 BEGIN;
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' INNER JOIN dbo.CompanyCustomers AS c ON c.ID = cd.CustomerID [ADDITIONAL_JOINS]')
	END;

	-- now remove the additional joins placeholder
	SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' ')


	-- and finally, execute the generated string
	-- use sp_executesql in order to correctly handle variables
	-- also, sp_executesql should re-use execution plans for each variation of the query

	PRINT @Statement

	EXEC sp_executesql 
		@Statement,
		N'@PersonnelIDs IDTable READONLY,
		  @IndustryIDs IDTable READONLY,
		  @CustomerTypeIDs IDTable READONLY,
		  @AccountTypeIDs IDTable READONLY,
		  @ProductIDs IDTable READONLY,
		  @CustomerIDs IDTable READONLY,
		  @StartDate DATE,
		  @EndDate DATE,
		  @NumMonths INT',
		@PersonnelIDs,
		@IndustryIDs,
		@CustomerTypeIDs,
		@AccountTypeIDs,
		@ProductIDs,
		@CustomerIDs,
		@StartDate,
		@EndDate,
		@NumMonths

	RETURN

-- /* 
END
-- */
GO
/****** Object:  StoredProcedure [ds].[DataByMonth_BackUp]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =========================================
-- Return monthly breakdown per the specified criteria
-- see in-prod notes for parameter details
-- =========================================
-- /* 
CREATE OR ALTER PROCEDURE [ds].[DataByMonth_BackUp](
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY,
	@IndustryIDs IDTable READONLY,
	@CustomerTypeIDs IDTable READONLY,
	@AccountTypeIDs IDTable READONLY,
	@ProductIDs IDTable READONLY,
	@CustomerIDs IDTable READONLY
	) AS 
BEGIN
-- */
	

	/* 
		PersonnelID list is required
		Other ID lists (IndustryIDs, CustomerTypeIDs etc) are optional
		If records are found in non-personnel lists,
		  then will be filtered accordingly, otherwise all matching
		  rows are included.
	*/

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable
	DECLARE @IndustryIDs IDTable
	DECLARE @CustomerTypeIDs IDTable
	DECLARE @AccountTypeIDs IDTable
	DECLARE @ProductIDs IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)

	-- INSERT INTO @IndustryIDs (ID) VALUES (6)
	-- INSERT INTO @IndustryIDs (ID) VALUES (7)
	-- INSERT INTO @IndustryIDs (ID) VALUES (9)

	INSERT INTO @ProductIDs (ID) VALUES (2)
	

	-- INSERT INTO @CustomerTypeIDs (ID) VALUES (3)
	-- INSERT INTO @CustomerTypeIDs (ID) VALUES (4)

	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;


	DECLARE @NumMonths INT
	SET @NumMonths = dbo.FullMonthsSeparation(@StartDate, @EndDate) + 1;
	
	
	DECLARE @HasFilters BIT
	DECLARE @FilterOnIndustries BIT
	DECLARE @FilterOnCustomerTypes BIT
	DECLARE @FilterOnAccountTypes BIT
	DECLARE @FilterOnProducts BIT
	DECLARE @FilterOnCustomers BIT
	
	SET @FilterOnIndustries = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @IndustryIDs)
	SET @FilterOnCustomerTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerTypeIDs)
	SET @FilterOnAccountTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @AccountTypeIDs)
	SET @FilterOnProducts = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @ProductIDs)
	SET @FilterOnCustomers = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerIDs)
	
	DECLARE @Statement NVARCHAR(2000)

	SET @Statement = 
		'SELECT data.*, periods.* FROM ( 
			SELECT 
				cd.DataPeriod, 
				SUM(COALESCE(cd.DataSalesActual, 0)) AS SalesActual, 
				SUM(COALESCE(cd.DataSalesForecast, 0)) AS SalesForecast,
				SUM(COALESCE(cd.DataSalesTarget, 0)) AS SalesTarget, 
				SUM(COALESCE(cd.DataCostActual, 0)) AS CostActual, 
				SUM(COALESCE(cd.DataCostForecast, 0)) AS CostForecast,
				SUM(COALESCE(cd.DataCostTarget, 0)) AS CostTarget,
				SUM(COALESCE(cd.DataCallsActual, 0)) AS CallsActual, 
				SUM(COALESCE(cd.DataCallsForecast, 0)) AS CallsForecast,
				SUM(COALESCE(cd.DataCallsTarget, 0)) AS CallsTarget,
				SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
				SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
				SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity
			FROM dbo.CustomerData AS cd 
			INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 

			[ADDITIONAL_JOINS] 

			WHERE cd.DataPeriod >= @StartDate 
				AND cd.DataPeriod <= @EndDate 
			GROUP BY cd.DataPeriod 
		
		) AS data 

		RIGHT JOIN ( 
		
			SELECT Number, DATEADD(MONTH, Number - 1, @StartDate) AS Period 
			FROM dbo.Numbers  
			WHERE Number <= @NumMonths 

		) AS periods ON periods.Period = data.DataPeriod 

		ORDER BY periods.Period'

	IF @FilterOnIndustries = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @IndustryIDs AS iid ON c.CustomerIndustryID = iid.ID ')
	END;

	IF @FilterOnCustomerTypes = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerTypeIDs AS ctids ON c.CustomerTypeID = ctids.ID ')
	END;

	IF @FilterOnAccountTypes = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @AccountTypeIDs AS atids ON c.CustomerAccountTypeID = atids.ID ')
	END;

	IF @FilterOnProducts = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @ProductIDs AS prodIDs ON cd.ProductID = prodIDs.ID ')
	END;

	IF @FilterOnCustomers = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerIDs AS custIDs ON cd.CustomerID = custIDs.ID ')
	END;

	-- if we applied additional filters, those will need the company customers joined to resolve
	IF @HasFilters = 1 BEGIN;
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' INNER JOIN dbo.CompanyCustomers AS c ON c.ID = cd.CustomerID [ADDITIONAL_JOINS]')
	END;

	-- now remove the additional joins placeholder
	SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' ')


	-- and finally, execute the generated string
	-- use sp_executesql in order to correctly handle variables
	-- also, sp_executesql should re-use execution plans for each variation of the query

	PRINT @Statement

	EXEC sp_executesql 
		@Statement,
		N'@PersonnelIDs IDTable READONLY,
		  @IndustryIDs IDTable READONLY,
		  @CustomerTypeIDs IDTable READONLY,
		  @AccountTypeIDs IDTable READONLY,
		  @ProductIDs IDTable READONLY,
		  @CustomerIDs IDTable READONLY,
		  @StartDate DATE,
		  @EndDate DATE,
		  @NumMonths INT',
		@PersonnelIDs,
		@IndustryIDs,
		@CustomerTypeIDs,
		@AccountTypeIDs,
		@ProductIDs,
		@CustomerIDs,
		@StartDate,
		@EndDate,
		@NumMonths

	RETURN

-- /* 
END
-- */
GO
/****** Object:  StoredProcedure [ds].[GetCallOverviewBySalespersons]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE OR ALTER PROCEDURE [ds].[GetCallOverviewBySalespersons](  
 @FiscalYear int,  
 @PersonnelIDs IDTable READONLY  
) AS  
  
BEGIN  
  
  
 /* Test Data */  
 /*DECLARE @PersonnelIDs dbo.IDTable  
 INSERT INTO @PersonnelIDs (ID) VALUES (38)  
 INSERT INTO @PersonnelIDs (ID) VALUES (39)  
 INSERT INTO @PersonnelIDs (ID) VALUES (9)  
  
 DECLARE @FiscalYear int  
 SET @FiscalYear = 2016*/  
  
  
 Select   
  cat.ID as TypeID,   
  cat.AccountTypeName as TypeName,   
  --count(*) as NumberOfAccounts,	-- OLD
  count(cc.ID) as NumberOfAccounts,   -- NEW
  SUM(cat.AccountTypeCallsPerMonthGoal) CallGoal,   
  SUM(cdf.dataCalls) CallPlan,  
  SUM(cdf.dataSales) SalesForecast  
 From CompanyAccountTypes cat  
 Left Join CompanyCustomers cc  
 ON cc.CustomerAccountTypeID = cat.ID  
  
 Left Join (Select   
     CustomerID,  
     SUM(COALESCE([DataCallsForecast], 0)) dataCalls,  
     SUM(COALESCE([DataSalesForecast], 0)) dataSales  
    From CustomerDataWithFiscal  
    Inner Join @PersonnelIDs AS pids  
     ON pids.ID = CustomerDataWithFiscal.PersonnelID  
    Where CustomerDataWithFiscal.FiscalYear = @fiscalYear  
    Group By CustomerID) as cdf  
 On cdf.CustomerID = cc.ID  
  
 Group By cat.ID, cat.AccountTypeName  
  
RETURN  
END  
GO
/****** Object:  StoredProcedure [ds].[GetOpportunitiesByAccountType]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






-- =========================================
-- Get Opportunities (master sproc for all analysis opps)
-- =========================================

-- /*
CREATE OR ALTER PROCEDURE [ds].[GetOpportunitiesByAccountType] (
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY
	) AS 
BEGIN
-- */

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)
	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;

	SELECT
		cat.ID AS GroupEntityID,
		cat.AccountTypeName AS GroupEntityName,
		SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
		SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
		SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity,
		SUM(COALESCE(cd.DataSalesForecast, 0)) AS Forecast,
		--(SUM(COALESCE(cd.MarketShare, 0))) AS MarketShare 
		CASE 
            WHEN SUM(COALESCE(cd.DataPotential, 0)) = 0 THEN 0 
            ELSE (SUM(COALESCE(cd.DataSalesForecast, 0)) * 100.0 / SUM(COALESCE(cd.DataPotential, 0))) 
        END AS MarketShare
	
	FROM dbo.CustomerData AS cd
	INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 
	INNER JOIN dbo.CompanyCustomers AS cc ON cc.ID = cd.CustomerID
	INNER JOIN dbo.CompanyAccountTypes AS cat ON cat.ID = cc.CustomerAccountTypeID
	
	
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate

	GROUP BY
		cat.ID,
		cat.AccountTypeName

	RETURN

-- /* 
END
-- */

GO
/****** Object:  StoredProcedure [ds].[GetOpportunitiesByCustomer]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =========================================
-- Get Opportunities (master sproc for all analysis opps)
-- =========================================

-- /*

CREATE OR ALTER PROCEDURE [ds].[GetOpportunitiesByCustomer] (
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY
	) AS 
BEGIN
-- */

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)
	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;

	SELECT
		cc.ID AS GroupEntityID,
		cc.CustomerName AS GroupEntityName,
		SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
		SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
		SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity,
		SUM(COALESCE(cd.DataSalesForecast, 0)) AS Forecast,
		--(SUM(COALESCE(cd.MarketShare, 0))) AS MarketShare 
		CASE 
            WHEN SUM(COALESCE(cd.DataPotential, 0)) = 0 THEN 0 
            ELSE (SUM(COALESCE(cd.DataSalesForecast, 0)) * 100.0 / SUM(COALESCE(cd.DataPotential, 0))) 
        END AS MarketShare
	
	FROM dbo.CustomerData AS cd
	INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 
	INNER JOIN dbo.CompanyCustomers AS cc ON cc.ID = cd.CustomerID	
	
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate

	GROUP BY
		cc.ID,
		cc.CustomerName

	RETURN

-- /* 
END
-- */
GO
/****** Object:  StoredProcedure [ds].[GetOpportunitiesByCustomerType]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





-- =========================================
-- Get Opportunities (master sproc for all analysis opps)
-- =========================================

-- /*
CREATE OR ALTER PROCEDURE [ds].[GetOpportunitiesByCustomerType] (
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY
	) AS 
BEGIN
-- */

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)
	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;	

	SELECT
		cct.ID AS GroupEntityID,
		cct.TypeName AS GroupEntityName,
		SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
		SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
		SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity,
		SUM(COALESCE(cd.DataSalesForecast, 0)) AS Forecast,
		--(SUM(COALESCE(cd.MarketShare, 0))) AS MarketShare 
		CASE 
            WHEN SUM(COALESCE(cd.DataPotential, 0)) = 0 THEN 0 
            ELSE (SUM(COALESCE(cd.DataSalesForecast, 0)) * 100.0 / SUM(COALESCE(cd.DataPotential, 0))) 
        END AS MarketShare 
	
	FROM dbo.CustomerData AS cd
	INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 
	INNER JOIN dbo.CompanyCustomers AS cc ON cc.ID = cd.CustomerID
	INNER JOIN dbo.CompanyCustomerTypes AS cct ON cct.ID = cc.CustomerTypeID
	
	
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate

	GROUP BY
		cct.ID,
		cct.TypeName

	RETURN

-- /* 
END
-- */

GO
/****** Object:  StoredProcedure [ds].[GetOpportunitiesByIndustry]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






-- =========================================
-- Get Opportunities (master sproc for all analysis opps)
-- =========================================

-- /*
CREATE OR ALTER PROCEDURE [ds].[GetOpportunitiesByIndustry] (
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY
	) AS 
BEGIN
-- */

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)
	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;	

	SELECT
		ind.ID AS GroupEntityID,
		ind.IndustryName AS GroupEntityName,
		SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
		SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
		SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity,
		SUM(COALESCE(cd.DataSalesForecast, 0)) AS Forecast,
		--(SUM(COALESCE(cd.MarketShare, 0))) AS MarketShare 
		CASE 
            WHEN SUM(COALESCE(cd.DataPotential, 0)) = 0 THEN 0 
            ELSE (SUM(COALESCE(cd.DataSalesForecast, 0)) * 100.0 / SUM(COALESCE(cd.DataPotential, 0))) 
        END AS MarketShare 
	
	FROM dbo.CustomerData AS cd
	INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 
	INNER JOIN dbo.CompanyCustomers AS cc ON cc.ID = cd.CustomerID
	INNER JOIN dbo.CompanyIndustries AS ind ON ind.ID = cc.CustomerIndustryID
	
	
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate

	GROUP BY
		ind.ID,
		ind.IndustryName

	RETURN

-- /* 
END
-- */

GO
/****** Object:  StoredProcedure [ds].[GetOpportunitiesBySalespersons]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =========================================
-- Get Opportunities (master sproc for all analysis opps)
-- =========================================

-- /*
CREATE OR ALTER PROCEDURE [ds].[GetOpportunitiesBySalespersons] (
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY
	) AS 
BEGIN
-- */

	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable

	SET @StartDate = '2015-06-01'
	SET @EndDate = '2016-05-01'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)
	-- */ -- END TEST DATA

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;

	SELECT
		p.ID AS GroupEntityID,
		p.PersonFirstName + ' ' + p.PersonLastName AS GroupEntityName,
		SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
		SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
		SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity,
		SUM(COALESCE(cd.DataSalesForecast, 0)) AS Forecast,
		--(SUM(COALESCE(cd.MarketShare, 0))) AS MarketShare 
		CASE 
            WHEN SUM(COALESCE(cd.DataPotential, 0)) = 0 THEN 0 
            ELSE (SUM(COALESCE(cd.DataSalesForecast, 0)) * 100.0 / SUM(COALESCE(cd.DataPotential, 0))) 
        END AS MarketShare 
	
	FROM dbo.CustomerData AS cd
	INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID 
	INNER JOIN dbo.Personnel AS p ON p.ID = pids.ID
	
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate

	GROUP BY
		p.ID,
		p.PersonFirstName + ' ' + p.PersonLastName

	RETURN

-- /* 
END
-- */

GO
/****** Object:  StoredProcedure [ds].[GetTopBottomMatrix]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [ds].[GetTopBottomMatrix](@StartDate DATETIME2, @EndDate DATETIME2, @PersonnelIDs IDTable READONLY) AS 	

	DECLARE @Today DATE = GETDATE();
    DECLARE @DayOfYear INT = DATEPART(DAYOFYEAR, @Today);
    DECLARE @DaysInYear INT = CASE WHEN YEAR(@Today) % 4 = 0 AND (YEAR(@Today) % 100 != 0 OR YEAR(@Today) % 400 = 0) THEN 366 ELSE 365 END;
		

		SELECT dsA.*
		FROM (

			SELECT TOP 100 PERCENT 
				'Customer' AS Entity,
				'Sales' AS Factor,
				'Top' AS Mode,
				cd.CustomerID AS EntityID,
				cc.CustomerName AS EntityName,				
				--ISNULL(SUM(cd.DataSalesActual) / @DaysInYear * @DayOfYear,0) AS YTDActual,
				ISNULL(SUM(cd.DataSalesActual),0) AS YTDActual,
				ISNULL(SUM(cd.DataSalesForecast) / @DaysInYear * @DayOfYear,0) AS YTDForecast,
				0.0 AS YTDDifference
	
			FROM dbo.CustomerDataWithFiscal AS cd
			INNER JOIN dbo.CompanyCustomers AS cc ON cc.ID = cd.CustomerID
			INNER JOIN @PersonnelIDs AS pid ON pid.ID = cd.PersonnelID

			WHERE cd.DataPeriod >= @StartDate 
				AND cd.DataPeriod <= @EndDate
			GROUP BY 
		
				cd.CustomerID,
				cc.CustomerName

			ORDER BY SUM(cd.DataSalesActual) DESC

		) AS dsA

		UNION ALL

		SELECT dsB.*
		FROM (

			SELECT TOP 100 PERCENT 
				'Salesperson' AS Entity,
				'Sales' AS Factor,
				'Top' AS Mode,
				p.ID AS EntityID,
				p.PersonFirstName + ' ' + p.PersonLastName AS EntityName,
				--ISNULL(SUM(cd.DataSalesActual) / @DaysInYear * @DayOfYear,0) AS YTDActual,
				ISNULL(SUM(cd.DataSalesActual),0) AS YTDActual,
				ISNULL(SUM(cd.DataSalesForecast) / @DaysInYear * @DayOfYear,0) AS YTDForecast,
				0.0 AS YTDDifference
	
			FROM dbo.CustomerDataWithFiscal AS cd
			INNER JOIN dbo.CompanyCustomers AS cc ON cd.CustomerID = cc.ID
			INNER JOIN dbo.CompanyCustomersPersonnel AS cp ON cp.CustomerID = cc.ID
			INNER JOIN dbo.Personnel AS p ON p.ID = cp.PersonnelID
			INNER JOIN @PersonnelIDs AS pid ON pid.ID = p.ID

			WHERE cd.DataPeriod >= @StartDate 
				AND cd.DataPeriod <= @EndDate
			GROUP BY 
		
				p.ID,
				p.PersonFirstName + ' ' + p.PersonLastName

			ORDER BY SUM(cd.DataSalesActual) DESC

		) AS dsB
		
	

GO
/****** Object:  StoredProcedure [ds].[HistoricSales]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* dym:SQLSource Version 1.0
---------------------------------------------------------------------
dym:TargetStartingVersion: 0.9.5.0
dym:TargetEndingVersion: 0.9.6.0
---------------------------------------------------------------------

---------------------------------------------------------------------*/


-- =========================================
-- Return monthly breakdown per the specified criteria
-- see in-prod notes for parameter details
-- (add AccountType and ProductID handling, convert to dynamic build)
-- =========================================
-- /* 
CREATE OR ALTER PROCEDURE [ds].[HistoricSales](
	@StartDate DATE, 
	@EndDate DATE, 
	@PersonnelIDs IDTable READONLY,
	@IndustryIDs IDTable READONLY,
	@CustomerTypeIDs IDTable READONLY,
	@AccountTypeIDs IDTable READONLY,
	@ProductIDs IDTable READONLY,
	@CustomerIDs IDTable READONLY
	) AS 
BEGIN
-- */
	

	/* 
		PersonnelID list is required
		IndustryIDs and CustomerTypeIDs are optional
		If records are found in Industry and CustomerType lists,
		  then will be filtered accordingly, otherwise all matching
		  rows are included.
	*/

	 /* TEST DATA

	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @PersonnelIDs dbo.IDTable
	DECLARE @IndustryIDs IDTable
	DECLARE @CustomerTypeIDs IDTable
	DECLARE @AccountTypeIDs IDTable
	DECLARE @ProductIDs IDTable

	SET @StartDate = '2013-01-01'
	SET @EndDate = '2016-12-31'
	INSERT INTO @PersonnelIDs (ID) VALUES (38)
	INSERT INTO @PersonnelIDs (ID) VALUES (39)
	INSERT INTO @PersonnelIDs (ID) VALUES (9)

	--INSERT INTO @IndustryIDs (ID) VALUES (6)
	--INSERT INTO @IndustryIDs (ID) VALUES (7)
	--INSERT INTO @IndustryIDs (ID) VALUES (9)
	
	--INSERT INTO @CustomerTypeIDs (ID) VALUES (3)
	--INSERT INTO @CustomerTypeIDs (ID) VALUES (4)


	-- */

	-- Verify they entered some personnel, if not throw an error
	DECLARE @HasPersonnel BIT
	SET @HasPersonnel = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @PersonnelIDs)

	IF @HasPersonnel = 0 BEGIN;
		THROW 51000, 'Procedure must be limited by personnel', 1;
	END;

	DECLARE @NumYears INT
	SET @NumYears = (dbo.FullMonthsSeparation(@StartDate, @EndDate) / 12) + 1;
	
	-- When we get our binding list of fiscal years from the numbers table, if the start date is a
	-- January, we need to kick that number back by one for the binding list to come out correctly.
	-- We'll track a new ref date for this...
	DECLARE @YearAddStartDate DATE
	IF MONTH(@StartDate) = 1 
		SET @YearAddStartDate = DATEADD(MONTH, -1, @StartDate) 
	ELSE 
		SET @YearAddStartDate = @StartDate

	
	DECLARE @HasFilters BIT
	DECLARE @FilterOnIndustries BIT
	DECLARE @FilterOnCustomerTypes BIT
	DECLARE @FilterOnAccountTypes BIT
	DECLARE @FilterOnProducts BIT
	DECLARE @FilterOnCustomers BIT
	SET @FilterOnIndustries = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @IndustryIDs)
	SET @FilterOnCustomerTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerTypeIDs)
	SET @FilterOnAccountTypes = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @AccountTypeIDs)
	SET @FilterOnProducts = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @ProductIDs)
	SET @FilterOnCustomers = (SELECT CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END FROM @CustomerIDs)

	DECLARE @Statement NVARCHAR(2000)

	SET @Statement = 
		'SELECT 
			periods.Period AS FiscalYear, 
			data.SalesActual, 
			data.SalesForecast, 
			data.CostActual, 
			data.CostForecast, 
			data.CallsActual, 
			data.CallsForecast,
			data.Potential,
			data.CurrentOpportunity,
			data.FutureOpportunity 
			
		FROM (

			SELECT
				cd.FiscalYear,
				SUM(COALESCE(cd.DataSalesActual, 0)) AS SalesActual,
				SUM(COALESCE(cd.DataSalesForecast, 0)) AS SalesForecast,
				SUM(COALESCE(cd.DataCostActual, 0)) AS CostActual,
				SUM(COALESCE(cd.DataCostForecast, 0)) AS CostForecast,
				SUM(COALESCE(cd.DataCallsActual, 0)) AS CallsActual,
				SUM(COALESCE(cd.DataCallsForecast, 0)) AS CallsForecast,
				SUM(COALESCE(cd.DataPotential, 0)) AS Potential,
				SUM(COALESCE(cd.DataCurrentOpportunity, 0)) AS CurrentOpportunity,
				SUM(COALESCE(cd.DataFutureOpportunity, 0)) AS FutureOpportunity
				
			FROM dbo.CustomerDataWithFiscal AS cd
			INNER JOIN dbo.CompanyCustomers AS c ON c.ID = cd.CustomerID
			INNER JOIN @PersonnelIDs AS pids ON pids.ID = cd.PersonnelID

			[ADDITIONAL_JOINS]

			WHERE cd.DataPeriod >= @StartDate
				AND cd.DataPeriod <= @EndDate
			GROUP BY cd.FiscalYear
		
		) AS data
		RIGHT JOIN (
	
			SELECT Number, YEAR(DATEADD(YEAR, Number, @YearAddStartDate)) AS Period
			FROM dbo.Numbers 
			WHERE Number <= @NumYears

		) AS periods ON periods.Period = data.FiscalYear

		ORDER BY periods.Period'

	IF @FilterOnIndustries = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @IndustryIDs AS iid ON c.CustomerIndustryID = iid.ID ')
	END;

	IF @FilterOnCustomerTypes = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerTypeIDs AS ctids ON c.CustomerTypeID = ctids.ID ')
	END;

	IF @FilterOnAccountTypes= 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @AccountTypeIDs AS atids ON c.CustomerAccountTypeID = atids.ID ')
	END;

	IF @FilterOnProducts = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @ProductIDs AS pdids ON cd.ProductID = pdids.ID ')
	END;

		IF @FilterOnCustomers = 1 BEGIN;
		SET @HasFilters = 1
		SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' [ADDITIONAL_JOINS] INNER JOIN @CustomerIDs AS custids ON cd.CustomerID = custids.ID ')
	END;

	-- now remove the additional joins placeholder
	SET @Statement = REPLACE(@Statement, '[ADDITIONAL_JOINS]', ' ')


	-- and finally, execute the generated string
	-- use sp_executesql in order to correctly handle variables
	-- also, sp_executesql should re-use execution plans for each variation of the query

	PRINT @Statement

	EXEC sp_executesql 
		@Statement,
		N'@PersonnelIDs IDTable READONLY,
		  @IndustryIDs IDTable READONLY,
		  @CustomerTypeIDs IDTable READONLY,
		  @AccountTypeIDs IDTable READONLY,
		  @ProductIDs IDTable READONLY,
		  @CustomerIDs IDTable READONLY,
		  @StartDate DATE,
		  @EndDate DATE,
		  @NumYears INT,
		  @YearAddStartDate DATE',
		@PersonnelIDs,
		@IndustryIDs,
		@CustomerTypeIDs,
		@AccountTypeIDs,
		@ProductIDs,
		@CustomerIDs,
		@StartDate,
		@EndDate,
		@NumYears,
		@YearAddStartDate

	RETURN


-- /*
END
-- */
GO
/****** Object:  StoredProcedure [meta].[UpdateVersion]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [meta].[UpdateVersion](@Version NVARCHAR(50)) AS
BEGIN
	UPDATE meta.DatabaseSettings SET SettingValue = @Version WHERE SettingName = 'DBVersion';
END
GO
/****** Object:  StoredProcedure [staging].[UpsertAccountTypesToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- ==============================
-- Upsert Account Types
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertAccountTypesToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
	-- use this to resolve companyID until we get multi-company
	-- handling worked in...
	DECLARE @CompanyID INT
	SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID)
	
	-- update matches
	UPDATE t SET
		t.AccountTypeName = s.TypeName,
		t.AccountTypeCallsPerMonthGoal = s.TypeCallsPerMonthGoal
	FROM staging.AccountTypes AS s
	INNER JOIN dbo.CompanyAccountTypes AS t ON t.ClientID = s.TypeClientID;
		
	-- insert new
	INSERT INTO dbo.CompanyAccountTypes (
		ClientID,
		CompanyID,
		AccountTypeName,
		AccountTypeCallsPerMonthGoal
	) SELECT
		f.TypeClientID,
		@CompanyID,
		f.TypeName,
		f.TypeCallsPerMonthGoal
	FROM (
		SELECT s.*
		FROM staging.AccountTypes AS s
		LEFT JOIN dbo.CompanyAccountTypes AS t ON t.ClientID = s.TypeClientID AND t.CompanyID = @CompanyID
		WHERE t.ID IS NULL
	) AS f;
	
GO
/****** Object:  StoredProcedure [staging].[UpsertAreaOfResponsibilitiesToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ==============================
-- Upsert Area of Responsibilities
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertAreaOfResponsibilitiesToLive] (@AccountID INT) AS	
	
begin

	declare @CompanyID int 
	set @CompanyID = (select top 1 ID from dbo.Companies where AccountID = @AccountID)

	-- update matches
	begin
		update t set t.CompanyID = @CompanyID, t.[Name] = s.[Name]
			from staging.AreaOfResponsibilities as s
			inner join dbo.CompanyAreaOfResponsibilities as t on t.ClientID = s.ClientID;
	end

	-- insert new
	begin
		insert into dbo.CompanyAreaOfResponsibilities(ClientID, CompanyID, [Name])
		select  b.ClientID, @CompanyID, b.[Name]
		from (select x.* from staging.AreaOfResponsibilities as x left join dbo.CompanyAreaOfResponsibilities as y on x.[Name] = y.[Name] where y.[Name] is null) as b
	end
end
	
GO
/****** Object:  StoredProcedure [staging].[UpsertCompaniesToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










-- ==============================
-- Upsert Companies
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertCompaniesToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */

	-- update matches
	UPDATE t SET
		t.CompanyFullName = s.CompanyFullName,
		t.CompanyName = s.CompanyName,
		t.CompanyCode = s.CompanyCode,
		t.CompanyFiscalMonthEnd = s.CompanyFiscalMonthEnd
	FROM staging.Companies AS s 
	INNER JOIN dbo.Companies AS t ON t.ClientID = s.CompanyClientID
	WHERE t.AccountID = @AccountID;
	
	-- insert new
	INSERT INTO dbo.Companies (
		ClientID,
		CompanyFullName,
		CompanyName,
		CompanyCode,
		CompanyFiscalMonthEnd,
		AccountID
	) SELECT
		f.CompanyClientID,
		f.CompanyFullName,
		f.CompanyName,
		f.CompanyCode,
		f.CompanyFiscalMonthEnd,
		@AccountID
	FROM (
		SELECT s.*
		FROM staging.Companies AS s
		LEFT JOIN dbo.Companies AS t ON t.ClientID = s.CompanyClientID AND t.AccountID = @AccountID
		WHERE t.ID IS NULL
	) AS f;
	
	-- handle parent companies
	
	UPDATE target SET
		target.ParentCompanyID = source.ParentID
	FROM (
		-- get a list of resolved IDs for each applicable staging record
		-- Returns TargetID, ParentID
		SELECT 
			t.ID AS TargetID,
			(	SELECT t2.ID 
				FROM dbo.Companies AS t2 
				INNER JOIN staging.Companies AS s2 ON t2.ClientID = s2.CompanyParentClientID 
				WHERE t2.AccountID = @AccountID
			) AS ParentID
		FROM staging.Companies AS s 
		INNER JOIN dbo.Companies AS t ON t.ClientID = s.CompanyClientID
		
		WHERE t.AccountID = @AccountID
			AND s.CompanyParentClientID IS NOT NULL
		
	) AS source
	INNER JOIN dbo.Companies AS target ON target.ID = source.TargetID;
		

GO
/****** Object:  StoredProcedure [staging].[UpsertCustomersToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







-- ==============================
-- Upsert Customers
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertCustomersToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */

	-- use this to resolve companyID until we get multi-company
	-- handling worked in...
	DECLARE @CompanyID INT
	SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID)
	
	-- update matches
	UPDATE t SET
		t.CompanyID = @CompanyID,
		t.CustomerName = s.CustomerName,
		t.CustomerAddress1 = s.CustomerAddress1,
		t.CustomerAddress2 = s.CustomerAddress2,
		t.CustomerCity = s.CustomerCity,
		t.CustomerState = s.CustomerState,
		t.CustomerCountry = s.CustomerCountry,
		t.CustomerPhone = s.CustomerPhone,
		t.CustomerPostalCode = s.CustomerPostalCode
	FROM staging.Customers AS s 
	INNER JOIN dbo.CompanyCustomers AS t ON t.ClientID = s.CustomerClientID;
	
	-- insert new
	INSERT INTO dbo.CompanyCustomers (
		ClientID,
		CompanyID,
		CustomerName,
		CustomerAddress1,
		CustomerAddress2,
		CustomerCity,
		CustomerState,
		CustomerCountry,
		CustomerPhone,
		CustomerPostalCode
	) SELECT
		f.CustomerClientID,
		@CompanyID,
		f.CustomerName,
		f.CustomerAddress1,
		f.CustomerAddress2,
		f.CustomerCity,
		f.CustomerState,
		f.CustomerCountry,
		f.CustomerPhone,
		f.CustomerPostalCode
	FROM (
		SELECT s.*
		FROM staging.Customers AS s
		LEFT JOIN dbo.CompanyCustomers AS t ON t.ClientID = s.CustomerClientID AND t.CompanyID = @CompanyID
		WHERE t.ID IS NULL
	) AS f;
	
	-- handle Classifications
	
	UPDATE t SET 
		t.CustomerTypeID = src.CustomerTypeID,
		t.CustomerIndustryID = src.IndustryTypeID,
		t.CustomerAccountTypeID = src.AccountTypeID
	FROM dbo.CompanyCustomers AS t
	INNER JOIN (
	
		SELECT
			s.CustomerClientID,
			cat.ID AS AccountTypeID,
			ct.ID AS CustomerTypeID,
			it.ID AS IndustryTypeID
		FROM staging.Customers AS s
		LEFT JOIN dbo.CompanyAccountTypes AS cat ON cat.ClientID = s.CustomerAccountTypeClientID AND cat.CompanyID = @CompanyID
		LEFT JOIN dbo.CompanyCustomerTypes AS ct ON ct.ClientID = s.CustomerTypeClientID AND ct.CompanyID = @CompanyID
		LEFT JOIN dbo.CompanyIndustries AS it ON it.ClientID = s.CustomerIndustryTypeClientID AND it.CompanyID = @CompanyID
		
	) AS src ON t.ClientID = src.CustomerClientID AND t.CompanyID = @CompanyID;
	
GO
/****** Object:  StoredProcedure [staging].[UpsertCustomerTypesToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- ==============================
-- Upsert Customer Types
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertCustomerTypesToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
	-- use this to resolve companyID until we get multi-company
	-- handling worked in...
	DECLARE @CompanyID INT
	SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID)
	
	-- update matches
	UPDATE t SET
		t.TypeName = s.TypeName
	FROM staging.CustomerTypes AS s
	INNER JOIN dbo.CompanyCustomerTypes AS t ON t.ClientID = s.TypeClientID;
		
	-- insert new
	INSERT INTO dbo.CompanyCustomerTypes (
		ClientID,
		CompanyID,
		TypeName
	) SELECT
		f.TypeClientID,
		@CompanyID,
		f.TypeName
	FROM (
		SELECT s.*
		FROM staging.CustomerTypes AS s
		LEFT JOIN dbo.CompanyCustomerTypes AS t ON t.ClientID = s.TypeClientID AND t.CompanyID = @CompanyID
		WHERE t.ID IS NULL
	) AS f;
	
GO
/****** Object:  StoredProcedure [staging].[UpsertForecastDataToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =======================
-- Create staging for customer monthly data
-- =======================

CREATE OR ALTER PROCEDURE [staging].[UpsertForecastDataToLive] (@AccountID INT) AS


	-- If ClientIDs can't be resolved, they'll be ignored
	-- (should probably report on any client IDs that can't be resolved...)

	 /* TEST
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
	
	-- Update all rows matched by Customer, Personnel and Period
	
	UPDATE t SET
		t.DataSalesForecast = s.DataSalesForecast,
		t.DataCostForecast = 
			CASE WHEN s.DataSalesForecast IS NULL 
				THEN NULL 
				ELSE s.DataSalesForecast - (s.DataSalesForecast * s.DataCostForecast * 0.01) 
			END,
		t.DataCallsForecast = s.DataCallsForecast,
		t.DataSalesTarget = s.DataSalesTarget
	
	FROM ( 
		-- get staging with IDs resolved and GPP calculated 
		
		SELECT
			a.DataCustomerClientID,
			b.ID AS CustomerID,
			a.DataPersonClientID,
			c.ID AS PersonnelID,
			a.DataSalesForecast,
			a.DataCostForecast,
			a.DataCallsForecast,
			a.DataPeriod,
			a.DataSalesTarget

		FROM staging.ForecastData AS a
		INNER JOIN dbo.CompanyCustomers AS b ON a.DataCustomerClientID = b.ClientID
		INNER JOIN dbo.Personnel AS c ON a.DataPersonClientID = c.ClientID
			
		
	) AS s
	
	INNER JOIN dbo.CustomerData AS t 
		ON	s.CustomerID = t.CustomerID AND
			s.PersonnelID = t.PersonnelID AND
			s.DataPeriod = t.DataPeriod
	
	-- these to get us access to the account ID so we can only update their accounts
	INNER JOIN dbo.CompanyCustomers AS cust ON cust.ID = t.CustomerID
	INNER JOIN dbo.Companies AS comp ON comp.ID = cust.CompanyID
	
	WHERE comp.AccountID = @AccountID;

	
	
	-- Insert rows that can't match by Customer, Personnel and Period
	
	INSERT INTO dbo.CustomerData (
		CustomerID, PersonnelID, DataSalesForecast, DataCostForecast, DataCallsForecast, DataPeriod, DataSalesTarget
	)

	SELECT
		s.CustomerID,
		s.PersonnelID,
		s.DataSalesForecast,
		CASE WHEN s.DataSalesForecast IS NULL 
			THEN NULL 
			ELSE s.DataSalesForecast - (s.DataSalesForecast * s.DataCostForecast * 0.01) 
		END AS DataCostForecast,
		s.DataCallsForecast,
		s.DataPeriod,
		s.DataSalesTarget

	FROM (
	
		-- resolve clientIDs to system IDs
		SELECT
			a.ID,
			a.DataCustomerClientID,
			b.ID AS CustomerID,
			a.DataPersonClientID,
			c.ID AS PersonnelID,
			a.DataSalesForecast,
			a.DataCostForecast,
			a.DataCallsForecast,
			a.DataPeriod,
			a.DataSalesTarget

		FROM staging.ForecastData AS a
		INNER JOIN dbo.CompanyCustomers AS b ON a.DataCustomerClientID = b.ClientID
		INNER JOIN dbo.Personnel AS c ON a.DataPersonClientID = c.ClientID
			
	
	) AS s
		
	
	LEFT JOIN (
	
		SELECT cd.*
		FROM dbo.CustomerData AS cd

		-- these to get us access to the account ID so we can only update their accounts	
		INNER JOIN dbo.CompanyCustomers AS cust ON cust.ID = cd.CustomerID
		INNER JOIN dbo.Companies AS comp ON comp.ID = cust.CompanyID

	
	) AS t
	
		ON	s.CustomerID = t.CustomerID AND
			s.PersonnelID = t.PersonnelID AND
			s.DataPeriod = t.DataPeriod
	

	WHERE t.ID IS NULL;	-- not existing in target
	
	
	
	
GO
/****** Object:  StoredProcedure [staging].[UpsertIndustryTypesToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- ==============================
-- Upsert Industry Types
-- ==============================

CREATE OR ALTER PROCEDURE [staging].[UpsertIndustryTypesToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
begin

	declare @CompanyID int 
	set @CompanyID = (select top 1 ID from dbo.Companies where AccountID = @AccountID)

	-- update matches
	begin
		update t set  t.IndustryName = s.TypeName
			from staging.IndustryTypes as s
			inner join dbo.CompanyIndustries as t on t.ClientID = s.TypeClientID;
	end

	-- insert new
	begin
		insert into dbo.CompanyIndustries(ClientID, CompanyID, IndustryName)
		select  b.TypeClientID, @CompanyID, b.TypeName
		from (select x.* from staging.IndustryTypes as x left join dbo.CompanyIndustries as y on x.TypeName = y.IndustryName where y.IndustryName is null) as b
	end
end
	
GO
/****** Object:  StoredProcedure [staging].[UpsertMonthlyDataToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =======================
-- Create staging for customer monthly data
-- =======================

CREATE OR ALTER PROCEDURE [staging].[UpsertMonthlyDataToLive] (@AccountID INT) AS


	-- If ClientIDs can't be resolved, they'll be ignored
	-- (should probably report on any client IDs that can't be resolved...)

	 /* TEST
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
	
	-- Update all rows matched by Customer, Personnel and Period
	
	UPDATE t SET
		t.DataSalesActual = s.DataSalesActual,
		t.DataCostActual = 
			CASE WHEN s.DataSalesActual IS NULL 
				THEN NULL 
				ELSE s.DataSalesActual - (s.DataSalesActual * s.DataCostActual * 0.01) 
			END,
		t.DataCallsActual = s.DataCallsActual
	
	FROM ( 
		-- get staging with IDs resolved and GPP calculated 
		
		SELECT
			a.DataCustomerClientID,
			b.ID AS CustomerID,
			a.DataPersonClientID,
			c.ID AS PersonnelID,
			a.DataSalesActual,
			a.DataCostActual,
			a.DataCallsActual,
			a.DataPeriod

		FROM staging.CustomerData AS a
		INNER JOIN dbo.CompanyCustomers AS b ON a.DataCustomerClientID = b.ClientID
		INNER JOIN dbo.Personnel AS c ON a.DataPersonClientID = c.ClientID
			
		
	) AS s
	
	INNER JOIN dbo.CustomerData AS t 
		ON	s.CustomerID = t.CustomerID AND
			s.PersonnelID = t.PersonnelID AND
			s.DataPeriod = t.DataPeriod
	
	-- these to get us access to the account ID so we can only update their accounts
	INNER JOIN dbo.CompanyCustomers AS cust ON cust.ID = t.CustomerID
	INNER JOIN dbo.Companies AS comp ON comp.ID = cust.CompanyID
	
	WHERE comp.AccountID = @AccountID;

	
	
	-- Insert rows that can't match by Customer, Personnel and Period
	
	INSERT INTO dbo.CustomerData (
		CustomerID, PersonnelID, DataSalesActual, DataCostActual, DataCallsActual, DataPeriod
	)

	SELECT
		s.CustomerID,
		s.PersonnelID,
		s.DataSalesActual,
		CASE WHEN s.DataSalesActual IS NULL 
			THEN NULL 
			ELSE s.DataSalesActual - (s.DataSalesActual * s.DataCostActual * 0.01) 
		END AS DataCostActual,
		s.DataCallsActual,
		s.DataPeriod

	FROM (
	
		-- resolve clientIDs to system IDs
		SELECT
			a.ID,
			a.DataCustomerClientID,
			b.ID AS CustomerID,
			a.DataPersonClientID,
			c.ID AS PersonnelID,
			a.DataSalesActual,
			a.DataCostActual,
			a.DataCallsActual,
			a.DataPeriod

		FROM staging.CustomerData AS a
		INNER JOIN dbo.CompanyCustomers AS b ON a.DataCustomerClientID = b.ClientID
		INNER JOIN dbo.Personnel AS c ON a.DataPersonClientID = c.ClientID
			
	
	) AS s
		
	
	LEFT JOIN (
	
		SELECT cd.*
		FROM dbo.CustomerData AS cd

		-- these to get us access to the account ID so we can only update their accounts	
		INNER JOIN dbo.CompanyCustomers AS cust ON cust.ID = cd.CustomerID
		INNER JOIN dbo.Companies AS comp ON comp.ID = cust.CompanyID

	
	) AS t
	
		ON	s.CustomerID = t.CustomerID AND
			s.PersonnelID = t.PersonnelID AND
			s.DataPeriod = t.DataPeriod
	

	WHERE t.ID IS NULL;	-- not existing in target
	
	
	
	
GO
/****** Object:  StoredProcedure [staging].[UpsertPersonnelToLive]    Script Date: 03-11-2025 11:45:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- ==============================
-- Remove Gender from Personnel upsert
-- ==============================
CREATE OR ALTER PROCEDURE [staging].[UpsertPersonnelToLive] (@AccountID INT) AS

	 /* TEST DATA
	DECLARE @AccountID INT
	SET @AccountID = 1
	-- */
	
	-- use this to resolve companyID until we get multi-company
	-- handling worked in...
	DECLARE @CompanyID INT
	SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID)

	
	-- update matches
	UPDATE t SET
		--t.CompanyID = @CompanyID,
		t.PersonFirstName = s.PersonFirstName,
		t.PersonLastName = s.PersonLastName
	FROM staging.Personnel AS s
	INNER JOIN dbo.Personnel AS t ON t.ClientID = s.PersonClientID;
	
	
	-- insert new
	INSERT INTO dbo.Personnel (
		ClientID,
		CompanyID,
		PersonFirstName,
		PersonLastName
	) SELECT
		f.PersonClientID,
		@CompanyID,
		f.PersonFirstName,
		f.PersonLastName
	FROM (
		SELECT s.*
		FROM staging.Personnel AS s
		LEFT JOIN dbo.Personnel AS t ON t.ClientID = s.PersonClientID AND t.CompanyID = @CompanyID
		WHERE t.ID IS NULL
	) AS f;


	--SELECT * FROMPers
GO
