/****** Object:  StoredProcedure [staging].[UpsertAccountTypesToLive_Company]    Script Date: 09-10-2025 04:03:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ==============================
-- Upsert Account Types
-- ==============================

ALTER   PROCEDURE [staging].[UpsertAccountTypesToLive_Company] (@AccountID INT) AS

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
		t.AccountTypeName = s.TypeName,
		t.AccountTypeCallsPerMonthGoal = s.TypeCallsPerMonthGoal
		FROM staging.AccountTypes AS s
		INNER JOIN dbo.CompanyAccountTypes AS t 
		ON t.ClientID = s.TypeClientID
		AND t.CompanyID = @CompanyID; 
		
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
	
