

CREATE TABLE dbo.Users (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	AccountID INT NOT NULL REFERENCES dbo.Accounts(ID) ON UPDATE CASCADE ON DELETE CASCADE, -- soft ref dbo.Accounts(ID)
	PersonnelID INT, -- soft ref to dbo.Personnel,
	UserEmail NVARCHAR(128) NOT NULL,
	UserDisplayName NVARCHAR(30) NOT NULL
);
CREATE UNIQUE INDEX idxUserEmail ON dbo.Users (UserEmail);
CREATE UNIQUE INDEX idxUserAccountDisplayName ON dbo.Users(UserDisplayName, AccountID);
GO

CREATE SCHEMA aspnet;
GO

CREATE TABLE aspnet.UserMembership (
	ID INT NOT NULL PRIMARY KEY CLUSTERED REFERENCES dbo.Users (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	MemberPassword NVARCHAR(500) NOT NULL,
	MemberPasswordQuestion NVARCHAR(500),
	MemberPasswordAnswer NVARCHAR(128),
	MemberIsApproved BIT NOT NULL DEFAULT 0,
	MemberLastActivityDateUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberLastLoginDateUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberLastPasswordChangedDateUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberCreationDateUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberIsLockedOut BIT NOT NULL DEFAULT 0,
	MemberLastLockoutDateUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberFailedPasswordAttemptCount INT NOT NULL DEFAULT 0,
	MemberFailedPasswordWindowStartUTC DATETIME2 NOT NULL DEFAULT GETDATE(),
	MemberFailedPasswordAnswerAttemptCount INT NOT NULL DEFAULT 0,
	MemberFailedPasswordAnswerAttemptWindowStartUTC DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE aspnet.PermissionGroups (
	GroupName NVARCHAR(100) NOT NULL PRIMARY KEY,
	GroupDisplayName NVARCHAR(200) NOT NULL,
	GroupDescription NVARCHAR(2000)
);
GO

CREATE TABLE aspnet.OptionGroups (
	GroupName NVARCHAR(100) NOT NULL PRIMARY KEY,
	GroupDisplayName NVARCHAR(200) NOT NULL,
	GroupDescription NVARCHAR(2000)
);
GO

CREATE TABLE aspnet.Roles (
	RoleName NVARCHAR(100) NOT NULL PRIMARY KEY,
	PermissionGroupName NVARCHAR(100) NOT NULL REFERENCES aspnet.PermissionGroups (GroupName),
	RoleDisplayName NVARCHAR(200) NOT NULL,
	RoleDescription NVARCHAR(2000) NOT NULL
);
GO

CREATE TABLE aspnet.Options (
	OptionName NVARCHAR(100) NOT NULL PRIMARY KEY,
	OptionGroupName NVARCHAR(100) NOT NULL REFERENCES aspnet.OptionGroups (GroupName),
	OptionDisplayName NVARCHAR(200) NOT NULL,
	OptionDescription NVARCHAR(2000),
	OptionDefaultValue NVARCHAR(2000)
);
GO

CREATE TABLE aspnet.UserRoles (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	UserID INT NOT NULL REFERENCES dbo.Users (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	RoleName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE aspnet.UserOptions (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	UserID INT NOT NULL REFERENCES dbo.Users (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	OptionName NVARCHAR(100) NOT NULL,
	OptionValue NVARCHAR(2000)
);
GO


CREATE TABLE aspnet.UserLoginHistory (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,

	UserID INT NOT NULL REFERENCES dbo.Users (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	LoginHistoryActivity NVARCHAR(20) NOT NULL, -- 'Login', 'Logout', 'FailedAttempt', 'ChangedPassword', 'Lockout'
	LoginHistoryDateOfActionUTC DATETIME2,
	LoginHistoryNote NVARCHAR(200)
);



GO
CREATE VIEW aspnet.UserAspNetMembershipProviderInfo AS 
	SELECT
		u.ID,
		u.UserDisplayName,
		u.UserEmail,
        um.MemberPassword,
        um.MemberPasswordQuestion,
        um.MemberPasswordAnswer,
        um.MemberIsApproved,
        um.MemberLastActivityDateUTC,
        um.MemberLastLoginDateUTC,
        um.MemberLastPasswordChangedDateUTC,
        um.MemberCreationDateUTC,
        um.MemberIsLockedOut,
        um.MemberLastLockoutDateUTC,
        um.MemberFailedPasswordAttemptCount,
        um.MemberFailedPasswordWindowStartUTC,
        um.MemberFailedPasswordAnswerAttemptCount,
        um.MemberFailedPasswordAnswerAttemptWindowStartUTC
	FROM dbo.Users AS u
	INNER JOIN aspnet.UserMembership AS um ON u.ID = um.ID
;
GO



