DELETE FROM dbo.PersonnelHierarchy

DECLARE @AccountID INT
DECLARE @CompanyID INT
DECLARE @ParentPersonID INT

DECLARE @P1 INT
DECLARE @P2 INT
DECLARE @P3 INT
DECLARE @P4 INT
DECLARE @P5 INT
DECLARE @P6 INT
DECLARE @P7 INT
DECLARE @P8 INT

SET @AccountID = (SELECT TOP 1 ID FROM dbo.Accounts ORDER BY DateCreated DESC);
SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID ORDER BY DateCreated DESC)
SET @ParentPersonID = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID ORDER BY DateCreated DESC)


PRINT @AccountID
PRINT @CompanyID
PRINT @ParentPersonID

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P1', 'P1');
SET @P1 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P2', 'P2');
SET @P2 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P3', 'P3');
SET @P3 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P4', 'P4');
SET @P4 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P5', 'P5');
SET @P5 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P6', 'P6');
SET @P6 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P7', 'P7');
SET @P7 = SCOPE_IDENTITY()

INSERT INTO dbo.Personnel (CompanyID, PersonFirstName, PersonLastName)
	VALUES (@CompanyID, 'P8', 'P8');
SET @P8 = SCOPE_IDENTITY()



PRINT @P1 
PRINT @P2 
PRINT @P3 
PRINT @P4 
PRINT @P5 
PRINT @P6 
PRINT @P7 
PRINT @P8 

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P2, @ParentPersonID);
INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P2, @P1);

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P3, @P1);

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P4, @P2);
INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P4, @P3);

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P5, @P2);
INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P2, @P5);	-- create a circular ref

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P6, @P3);

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P7, @P4);
INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P7, @P5);

INSERT INTO dbo.PersonnelHierarchy (PersonnelID, ParentPersonnelID) VALUES (@P8, @P5);



