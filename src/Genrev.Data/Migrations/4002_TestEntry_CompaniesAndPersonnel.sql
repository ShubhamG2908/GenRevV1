-- Expect Personnel and Company info to be partially filled per 4001_TestEntry_DownstreamPersonnelHierarchy.sql

DECLARE @AccountID INT
DECLARE @CompanyID INT

DECLARE @Type0 INT
DECLARE @Type1 INT
DECLARE @Type2 INT
DECLARE @Type3 INT

DECLARE @Ind0 INT
DECLARE @Ind1 INT
DECLARE @Ind2 INT
DECLARE @Ind3 INT
DECLARE @Ind4 INT

DECLARE @Psn0 INT
DECLARE @Psn1 INT
DECLARE @Psn2 INT
DECLARE @Psn3 INT
DECLARE @Psn4 INT
DECLARE @Psn5 INT
DECLARE @Psn6 INT
DECLARE @Psn7 INT

DECLARE @Cust0 INT
DECLARE @Cust1 INT
DECLARE @Cust2 INT
DECLARE @Cust3 INT
DECLARE @Cust4 INT
DECLARE @Cust5 INT
DECLARE @Cust6 INT

SET @AccountID = (SELECT TOP 1 ID FROM dbo.Accounts ORDER BY DateCreated DESC)
SET @CompanyID = (SELECT TOP 1 ID FROM dbo.Companies WHERE AccountID = @AccountID ORDER BY DateCreated DESC)

INSERT INTO dbo.CompanyCustomerTypes (CompanyID, TypeName) VALUES (@CompanyID, 'Dist');
SET @Type0 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomerTypes (CompanyID, TypeName) VALUES (@CompanyID, 'OEM');
SET @Type1 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomerTypes (CompanyID, TypeName) VALUES (@CompanyID, 'Potential');
SET @Type2 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomerTypes (CompanyID, TypeName) VALUES (@CompanyID, 'Rep');
SET @Type3 = SCOPE_IDENTITY()


INSERT INTO dbo.CompanyIndustries (CompanyID, IndustryName) VALUES (@CompanyID, 'Auto');
SET @Ind0 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyIndustries (CompanyID, IndustryName) VALUES (@CompanyID, 'Builders');
SET @Ind1 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyIndustries (CompanyID, IndustryName) VALUES (@CompanyID, 'Dental');
SET @Ind2 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyIndustries (CompanyID, IndustryName) VALUES (@CompanyID, 'Developers');
SET @Ind3 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyIndustries (CompanyID, IndustryName) VALUES (@CompanyID, 'Distributor');
SET @Ind4 = SCOPE_IDENTITY()

SET @Psn0 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P1');
SET @Psn1 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P2');
SET @Psn2 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P3');
SET @Psn3 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P4');
SET @Psn4 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P5');
SET @Psn5 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P6');
SET @Psn6 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P7');
SET @Psn7 = (SELECT TOP 1 ID FROM dbo.Personnel WHERE CompanyID = @CompanyID AND PersonFirstName = 'P8');


INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 0', @Type0, @Ind0);
SET @Cust0 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 1', NULL, @Ind2);
SET @Cust1 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 2', @Type1, @Ind1);
SET @Cust2 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 3', @Type1, @Ind3);
SET @Cust3 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 4', @Type2, @Ind4);
SET @Cust4 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 5', @Type0, NULL);
SET @Cust5 = SCOPE_IDENTITY()
INSERT INTO dbo.CompanyCustomers (CompanyID, CustomerName, CustomerTypeID, CustomerIndustryID)
	VALUES (@CompanyID, 'Customer 6', @Type3, @Ind4);
SET @Cust6 = SCOPE_IDENTITY()


INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust0, @Psn0);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust0, @Psn1);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust1, @Psn0);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust1, @Psn2);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust2, @Psn3);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust3, @Psn6);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust4, @Psn7);
INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust5, @Psn3);
-- INSERT INTO dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID) VALUES (@Cust6, @Psn0);	-- customer 6 omitted to leave null for testing


