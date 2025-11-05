USE [GenRev_03_11]
GO
/****** Object:  StoredProcedure [dbo].[CreateUser]    Script Date: 03-11-2025 14:13:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [dbo].[CreateUser]
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







EXEC CreateUser
    @Email = 'admin1@yourcompany.com',
    @Password = 'aDDI1qNMnkt6YFM2aT6/EiV4FrE=',
    @SecurityQuestion = 'What is your favorite color?',
    @SecurityAnswer = 'Blue',
    @FirstName = 'admin',
    @LastName = '1',
    @Gender = 'M',
    @CompanyName = 'Company 1',
    @CompanyCode = 'CC1',
    @FiscalMonthEnd = 12,
    @CountryCode = 'US'
