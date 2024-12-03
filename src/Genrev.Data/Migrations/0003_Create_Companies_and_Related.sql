


CREATE TABLE dbo.CompanyIndustries (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	IndustryName NVARCHAR(50) NOT NULL
);
CREATE UNIQUE INDEX idxCompanyIndustriesCompositeKey ON dbo.CompanyIndustries (CompanyID, IndustryName);
GO


CREATE TABLE dbo.CompanyCustomerTypes (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	TypeName NVARCHAR(50) NOT NULL
);
CREATE UNIQUE INDEX idxCompanyCustomerTypesCompositeKey ON dbo.CompanyCustomerTypes (CompanyID, TypeName);
GO

CREATE TABLE dbo.CompanyCustomers (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	CustomerName NVARCHAR(120) NOT NULL,
	CustomerTypeID INT, -- soft ref to company customer types
	CustomerIndustryID INT -- soft ref to company industries
);
CREATE UNIQUE INDEX idxCompanyCustomersCompositeKey ON dbo.CompanyCustomers (CompanyID, CustomerName);
GO


CREATE TABLE dbo.CompanyCustomersPersonnel (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CustomerID INT NOT NULL REFERENCES dbo.CompanyCustomers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	PersonnelID INT NOT NULL
);
CREATE UNIQUE INDEX idxCompanyCustomersPersonnelCompositeKey ON dbo.CompanyCustomersPersonnel (CustomerID, PersonnelID);
GO

