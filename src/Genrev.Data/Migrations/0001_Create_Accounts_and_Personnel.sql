CREATE TABLE dbo.Numbers(Number INT IDENTITY(1,1)) 
SET NOCOUNT ON
WHILE COALESCE(SCOPE_IDENTITY(), 0) < 100000
BEGIN 
    INSERT dbo.Numbers DEFAULT VALUES 
END
SET NOCOUNT OFF
ALTER TABLE dbo.Numbers ADD CONSTRAINT PK_Numbers PRIMARY KEY CLUSTERED (Number)
-- SELECT COUNT(*) FROM dbo.Numbers
GO


CREATE TABLE dbo.AccountStatuses(
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	EnumDesc NVARCHAR(50) NOT NULL
);
INSERT INTO dbo.AccountStatuses (ID, EnumDesc) VALUES (0, N'Default, not set');
INSERT INTO dbo.AccountStatuses (ID, EnumDesc) VALUES (1, N'Active');
INSERT INTO dbo.AccountStatuses (ID, EnumDesc) VALUES (2, N'Locked');
INSERT INTO dbo.AccountStatuses (ID, EnumDesc) VALUES (3, N'Inactive');
GO


CREATE TABLE dbo.AccountActivityCodes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED,
	EnumDesc NVARCHAR(100) NOT NULL
);
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (0, N'Created');
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (1, N'Locked');
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (2, N'Deactivated');
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (3, N'Unlocked');
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (4, N'Reactivated');
INSERT INTO dbo.AccountActivityCodes (ID, EnumDesc) VALUES (5, N'Deprovisioned');
GO



CREATE TABLE dbo.Accounts (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AccountStatus INT NOT NULL DEFAULT 0, -- soft lookup to dbo.AccountStatuses
	AccountEmail NVARCHAR(128) NOT NULL
);
CREATE UNIQUE INDEX idxAccountEmail ON dbo.Accounts (AccountEmail);
GO


CREATE TABLE dbo.AccountActivity (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AccountID INT NOT NULL REFERENCES dbo.Accounts (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ActivityCode INT NOT NULL, -- soft ref to dbo.AccountActivityCodes
	ActivityDate DATETIME2 NOT NULL,
	ActivityNote NVARCHAR(1000)
);
CREATE INDEX idxAccountActivityAccountFK ON dbo.AccountActivity (AccountID);
GO


CREATE TABLE dbo.Companies (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	AccountID INT NOT NULL REFERENCES dbo.Accounts (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ParentCompanyID INT, -- soft self-reference
	CompanyFullName NVARCHAR(256),
	CompanyName NVARCHAR(128) NOT NULL,
	CompanyCode NVARCHAR(12) NOT NULL,
	CompanyFiscalMonthEnd INT NOT NULL DEFAULT 12 -- month number, 1-12
);
CREATE INDEX idxCompanyAccountFK ON dbo.Companies (AccountID);
CREATE INDEX idxCompanyParentFK ON dbo.Companies (ParentCompanyID);
CREATE UNIQUE INDEX idxCompanyNamePerAccount ON dbo.Companies (AccountID, CompanyName);
CREATE UNIQUE INDEX idxCompanyCodePerAccount ON dbo.Companies (AccountID, CompanyCode);
GO



CREATE TABLE dbo.Personnel (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	PersonFirstName NVARCHAR(50) NOT NULL,
	PersonLastName NVARCHAR(50) NOT NULL,
	PersonGender NVARCHAR(1) DEFAULT N'M' -- M, F, or U (unknown)
);
CREATE INDEX idxPersonnelCompanyFK ON dbo.Personnel (CompanyID);
GO




CREATE TABLE dbo.Roles (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	RoleIsSysAdministrator BIT NOT NULL DEFAULT 0,	-- these two are set at time of company creation
	RoleIsSysSalesPro BIT NOT NULL DEFAULT 0,
	RoleName NVARCHAR(100) NOT NULL,
	RoleCode NVARCHAR(12) NOT NULL,
	RoleDescription NVARCHAR(500)
);
CREATE INDEX idxRoleCompanyFK ON dbo.Roles (CompanyID);
CREATE UNIQUE INDEX idxRoleNamePerCompany ON dbo.Roles (CompanyID, RoleName);
CREATE UNIQUE INDEX idxRoleCodePerCompany ON dbo.Roles (CompanyID, RoleCode);
GO



CREATE TABLE dbo.PersonnelRoles (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	PersonnelID INT NOT NULL, -- (soft) REFERENCES dbo.Personnel (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	RoleID INT NOT NULL REFERENCES dbo.Roles (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE INDEX idxPersonnelRolesPersonnelFK ON dbo.PersonnelRoles (PersonnelID);
CREATE INDEX idxPersonnelRolesRoleFK ON dbo.PersonnelRoles (RoleID);
CREATE UNIQUE INDEX idxPersonnelRolesUniqueComposite ON dbo.PersonnelRoles (PersonnelID, RoleID);
GO




CREATE TABLE dbo.PersonnelHierarchy (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	PersonnelID INT NOT NULL REFERENCES dbo.Personnel (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ParentPersonnelID INT NOT NULL -- soft ref to dbo.Personnel (ID)
);
CREATE INDEX idxPersonnelHierarchyPersonnelFK ON dbo.PersonnelHierarchy (PersonnelID);
CREATE INDEX idxPersonnelHierarchyParentPersonnelFK ON dbo.PersonnelHierarchy (ParentPersonnelID);
CREATE UNIQUE INDEX idxPersonnelHierarchyCompositeKey ON dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID);
GO



CREATE PROCEDURE dbo.GetDownstreamPersonnel (@PersonID INT) AS 
	
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
		p.PersonGender AS Gender 
	FROM dbo.Personnel AS p
	INNER JOIN (
		SELECT DISTINCT ID FROM @Results UNION SELECT @PersonID
	) AS h ON p.ID = h.ID

	RETURN

GO








CREATE PROCEDURE [dbo].[GetDownstreamPersonnelIDs] (@PersonID INT) AS 
	
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




