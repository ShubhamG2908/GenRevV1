	-- ==============================
	-- Upsert Account Types
	-- ==============================
	CREATE OR ALTER   PROCEDURE [staging].[UpsertAccountTypesToLive_Company] 
		@AccountID INT
		AS
		BEGIN
		SET NOCOUNT ON;


    DECLARE @CompanyID INT;
    SET @CompanyID = (
        SELECT TOP 1 ID 
        FROM dbo.Companies 
        WHERE AccountID = @AccountID
    );


    UPDATE t
    SET 
        t.AccountTypeName = s.TypeName,
        t.AccountTypeCallsPerMonthGoal = s.TypeCallsPerMonthGoal
    FROM staging.AccountTypes AS s
    INNER JOIN dbo.CompanyAccountTypes AS t
        ON t.ClientID = s.TypeClientID
       AND t.CompanyID = @CompanyID
	   WHERE 
    (t.AccountTypeName <> s.TypeName
     OR t.AccountTypeCallsPerMonthGoal <> s.TypeCallsPerMonthGoal);


    INSERT INTO dbo.CompanyAccountTypes (
        ClientID,
        CompanyID,
        AccountTypeName,
        AccountTypeCallsPerMonthGoal
    )
    SELECT
        s.TypeClientID,
        @CompanyID,
        s.TypeName,
        s.TypeCallsPerMonthGoal
    FROM staging.AccountTypes AS s
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.CompanyAccountTypes AS t
        WHERE t.ClientID = s.TypeClientID
          AND t.CompanyID = @CompanyID
    );
END
