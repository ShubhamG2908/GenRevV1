CREATE OR ALTER PROCEDURE CreateCompany
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
                DateCreated, AccountStatus, AccountEmail, AccountApiEnabled,
                AccountApiKey, AccountApiAllowIpBypass, AccountApiPassword
            )
            VALUES (
                GETDATE(), 1, @Email, 0, NULL, 0, @Password
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
            GETDATE(),
            '1900-01-01',
            0,
            0,
            '1900-01-01'
        );

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
    @Email = 'admin4@example.com',
    @Password = '123456789',
    @SecurityQuestion = 'What is your favorite color?',
    @SecurityAnswer = 'Blue',
    @FirstName = 'admin',
    @LastName = '4',
    @Gender = 'M',
    @CompanyName = 'Company 4',
    @CompanyCode = 'CC4',
    @FiscalMonthEnd = 12,
    @CountryCode = 'UK'
