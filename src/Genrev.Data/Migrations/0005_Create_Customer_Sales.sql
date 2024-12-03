CREATE TABLE dbo.CustomerSales (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	CustomerID INT NOT NULL REFERENCES dbo.CompanyCustomers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	SalesFirstDayOfMonth DATE NOT NULL,
	SalesDollars DECIMAL(19, 4),
	SalesGPP DECIMAL(5, 2),
	SalesForecastDollars DECIMAL(19, 4),
	SalesForecastGPP DECIMAL(5, 2)
);

CREATE INDEX idxCustomerSalesCustomerID ON dbo.CustomerSales (CustomerID);
CREATE UNIQUE INDEX idxCustomerSalesCustomerPeriod ON dbo.CustomerSales (CustomerID, SalesFirstDayOfMonth);
CREATE INDEX idxCustomerSalesFirstDayOfMonth ON dbo.CustomerSales (SalesFirstDayOfMonth);
GO






 CREATE PROCEDURE dbo.GetDownstreamCustomerIDs (@PersonID INT) AS BEGIN

	 /* TEST DATA
		DECLARE @PersonID INT
		SET @PersonID = 39
	-- */

	IF OBJECT_ID('tempdb..#DCIDPersonnel') IS NOT NULL 
		DROP TABLE #DCIDPersonnel;

	CREATE TABLE #DCIDPersonnel (
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





CREATE TABLE dbo.CompanyProductGroups (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ParentID INT NULL, -- self-ref for hierarchy
	GroupName NVARCHAR(50) NOT NULL
);
CREATE INDEX idxCompanyProductGroupsCompanyFK ON dbo.CompanyProductGroups (CompanyID);
CREATE UNIQUE INDEX idxCompanyProductGroupName ON dbo.CompanyProductGroups (CompanyID, GroupName);
GO




CREATE TABLE dbo.CompanyProducts (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CompanyID INT NOT NULL REFERENCES dbo.Companies (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	ProductSKU NVARCHAR(50) NOT NULL,
	ProductDescription NVARCHAR (255) NOT NULL,
	ProductGroupID INT NULL  -- REFERENCES dbo.CompanyProductGroups (ID) ON UPDATE CASCADE ON DELETE SET NULL
	
);
CREATE UNIQUE INDEX idxCompanyProductSKU ON dbo.CompanyProducts (CompanyID, ProductSKU);
CREATE INDEX idxCompanyProductsCompanyFK ON dbo.CompanyProducts (CompanyID);
CREATE INDEX idxCompanyProductsGroup ON dbo.CompanyProducts (ProductGroupID);
GO




-- map which customers of the company are being sold what products
CREATE TABLE dbo.CompanyCustomerProducts (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	ProductID INT NOT NULL, -- REFERENCES dbo.CompanyProducts (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	CustomerID INT NOT NULL REFERENCES dbo.CompanyCustomers (ID) ON UPDATE CASCADE ON DELETE CASCADE
);
CREATE INDEX idxCompanyCustomerProductsProductFK ON dbo.CompanyCustomerProducts (ProductID);
CREATE INDEX idxCompanyCustomerProductsCustomerFK ON dbo.CompanyCustomerProducts (CustomerID);
CREATE UNIQUE INDEX idxCompanyCustomerProductsProdCust ON dbo.CompanyCustomerProducts (ProductID, CustomerID);
GO






CREATE TABLE dbo.CustomerData (
	ID INT NOT NULL PRIMARY KEY CLUSTERED IDENTITY(1, 1),
	DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
	rv ROWVERSION,
	
	CustomerID INT NOT NULL REFERENCES dbo.CompanyCustomers (ID) ON UPDATE CASCADE ON DELETE CASCADE,
	
	-- optional breakdowns, non-normalized for heavy read/analytics use
	PersonnelID INT, -- REFERENCES dbo.Personnel (ID) ON UPDATE CASCADE ON DELETE SET NULL,
	ProductID INT, -- REFERENCES dbo.CompanyProducts (ID) ON UPDATE CASCADE ON DELETE SET NULL,
		
	-- monthly, store first date of target month (e.g., 2016/12/01, 2015/09/01, etc.)
	DataPeriod DATE NOT NULL,	
	
	-- data: sales, costs, calls etc.
	DataSalesActual DECIMAL(19, 4),
	DataSalesForecast DECIMAL(19, 4),
	
	DataCostActual DECIMAL(19, 4),
	DataCostForecast DECIMAL(19, 4),
	
	DataCallsActual INT,
	DataCallsForecast INT
);
CREATE UNIQUE INDEX idxCustomerSalesMonth ON dbo.CustomerData (CustomerID, DataPeriod);
-- individual FK fields
CREATE INDEX idxCustomerDataCustomerFK ON dbo.CustomerData (CustomerID);
CREATE INDEX idxCustomerDataPersonnelFK ON dbo.CustomerData (PersonnelID) WHERE PersonnelID IS NOT NULL;
CREATE INDEX idxCustomerDataProductFK ON dbo.CustomerData (ProductID) WHERE ProductID IS NOT NULL;
-- compound FK indexes (if FK value not null)
CREATE INDEX idxCustomerDataCompanyPerson ON dbo.CustomerData (CustomerID, PersonnelID) WHERE PersonnelID IS NOT NULL;
CREATE INDEX idxCustomerDataCompanyProduct ON dbo.CustomerData (CustomerID, ProductID) WHERE ProductID IS NOT NULL;
CREATE INDEX idxCustomerDataCompanyPersonProduct ON dbo.CustomerData (CustomerID, PersonnelID, ProductID) WHERE PersonnelID IS NOT NULL AND ProductID IS NOT NULL;
GO






INSERT INTO dbo.CustomerData (CustomerID, DataPeriod, DataSalesActual, DataSalesForecast, DataCostActual, DataCostForecast)
SELECT
	cs.CustomerID,
	cs.SalesFirstDayOfMonth,
	cs.SalesDollars,
	cs.SalesForecastDollars,
	cs.SalesCost,
	cs.SalesCostForecast
FROM dbo.CustomerSales AS cs;
GO




DROP TABLE dbo.CustomerSales;
GO





-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by customer
-- Update to read from CustomerData instead of CustomerSales
-- =========================================
-- /* 
ALTER PROCEDURE [ds].[SalesByCustomer](
	@StartDate DATE, 
	@EndDate DATE, 
	@CustomerIDs CIDTable READONLY
	) AS 
BEGIN
-- */
	
	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @CustomerIDs CIDTable

	SET @StartDate = '2014-06-01'
	SET @EndDate = '2015-05-01'
	INSERT INTO @CustomerIDs (ID) VALUES (4)
	INSERT INTO @CustomerIDs (ID) VALUES (5)
	INSERT INTO @CustomerIDs (ID) VALUES (6)
	INSERT INTO @CustomerIDs (ID) VALUES (7)
	INSERT INTO @CustomerIDs (ID) VALUES (8)
	INSERT INTO @CustomerIDs (ID) VALUES (9)
	INSERT INTO @CustomerIDs (ID) VALUES (10)
	INSERT INTO @CustomerIDs (ID) VALUES (11)
	INSERT INTO @CustomerIDs (ID) VALUES (12)
	INSERT INTO @CustomerIDs (ID) VALUES (13)
	INSERT INTO @CustomerIDs (ID) VALUES (14)
	INSERT INTO @CustomerIDs (ID) VALUES (15)
	INSERT INTO @CustomerIDs (ID) VALUES (16)
	-- */ -- END TEST DATA

	SELECT
		cd.CustomerID,
		cc.CustomerName,
		SUM(cd.DataSalesActual) AS SumOfSalesDollars,
		SUM(cd.DataSalesForecast) AS SumOfSalesForecastDollars,
		SUM(cd.DataCostActual) AS SumOfSalesCost,
		SUM(cd.DataCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerData AS cd
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cd.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cd.CustomerID = cc.ID
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate
	GROUP BY cd.CustomerID, cc.CustomerName
	ORDER BY cc.CustomerName
	RETURN
	
-- /* 
END
-- */


GO





-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by customer
-- Update to read from CustomerData instead of CustomerSales
-- =========================================
-- /* 
ALTER PROCEDURE [ds].[SalesByCustomerType](
	@StartDate DATE, 
	@EndDate DATE, 
	@CustomerIDs CIDTable READONLY
	) AS 
BEGIN
-- */
	
	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @CustomerIDs CIDTable

	SET @StartDate = '2014-06-01'
	SET @EndDate = '2015-05-01'
	INSERT INTO @CustomerIDs (ID) VALUES (4)
	INSERT INTO @CustomerIDs (ID) VALUES (5)
	INSERT INTO @CustomerIDs (ID) VALUES (6)
	INSERT INTO @CustomerIDs (ID) VALUES (7)
	INSERT INTO @CustomerIDs (ID) VALUES (8)
	INSERT INTO @CustomerIDs (ID) VALUES (9)
	INSERT INTO @CustomerIDs (ID) VALUES (10)
	INSERT INTO @CustomerIDs (ID) VALUES (11)
	INSERT INTO @CustomerIDs (ID) VALUES (12)
	INSERT INTO @CustomerIDs (ID) VALUES (13)
	INSERT INTO @CustomerIDs (ID) VALUES (14)
	INSERT INTO @CustomerIDs (ID) VALUES (15)
	INSERT INTO @CustomerIDs (ID) VALUES (16)
	-- */ -- END TEST DATA

	SELECT
		cc.CustomerTypeID,
		ct.TypeName,
		SUM(cd.DataSalesActual) AS SumOfSalesDollars,
		SUM(cd.DataSalesForecast) AS SumOfSalesForecastDollars,
		SUM(cd.DataCostActual) AS SumOfSalesCost,
		SUM(cd.DataCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerData AS cd
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cd.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cd.CustomerID = cc.ID
	INNER JOIN dbo.CompanyCustomerTypes AS ct ON ct.ID = cc.CustomerTypeID
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate
	GROUP BY cc.CustomerTypeID, ct.TypeName
	ORDER BY ct.TypeName
	RETURN
	
-- /* 
END
-- */


GO









-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by industry
-- Update to read from CustomerData instead of CustomerSales
-- =========================================
-- /* 
ALTER PROCEDURE [ds].[SalesByIndustry](
	@StartDate DATE, 
	@EndDate DATE, 
	@CustomerIDs CIDTable READONLY
	) AS 
BEGIN
-- */
	
	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @CustomerIDs CIDTable

	SET @StartDate = '2014-06-01'
	SET @EndDate = '2015-05-01'
	INSERT INTO @CustomerIDs (ID) VALUES (4)
	INSERT INTO @CustomerIDs (ID) VALUES (5)
	INSERT INTO @CustomerIDs (ID) VALUES (6)
	INSERT INTO @CustomerIDs (ID) VALUES (7)
	INSERT INTO @CustomerIDs (ID) VALUES (8)
	INSERT INTO @CustomerIDs (ID) VALUES (9)
	INSERT INTO @CustomerIDs (ID) VALUES (10)
	INSERT INTO @CustomerIDs (ID) VALUES (11)
	INSERT INTO @CustomerIDs (ID) VALUES (12)
	INSERT INTO @CustomerIDs (ID) VALUES (13)
	INSERT INTO @CustomerIDs (ID) VALUES (14)
	INSERT INTO @CustomerIDs (ID) VALUES (15)
	INSERT INTO @CustomerIDs (ID) VALUES (16)
	-- */ -- END TEST DATA

	SELECT
		cc.CustomerIndustryID,
		ci.IndustryName,
		SUM(cd.DataSalesActual) AS SumOfSalesDollars,
		SUM(cd.DataSalesForecast) AS SumOfSalesForecastDollars,
		SUM(cd.DataCostActual) AS SumOfSalesCost,
		SUM(cd.DataCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerData AS cd
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cd.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cd.CustomerID = cc.ID
	INNER JOIN dbo.CompanyIndustries AS ci ON ci.ID = cc.CustomerIndustryID
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate
	GROUP BY cc.CustomerIndustryID, ci.IndustryName
	ORDER BY ci.IndustryName
	RETURN
	
-- /* 
END
-- */


GO






USE [grthree_data_dev]
GO

/****** Object:  StoredProcedure [ds].[SalesByMonth]    Script Date: 9/12/2016 5:38:07 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by month
-- =========================================
-- /* 
ALTER PROCEDURE [ds].[SalesByMonth](
	@StartDate DATE, 
	@EndDate DATE, 
	@CustomerIDs CIDTable READONLY
	) AS 
BEGIN
-- */
	
	 /* TEST DATA
	DECLARE @StartDate DATE
	DECLARE @EndDate DATE
	DECLARE @CustomerIDs CIDTable

	SET @StartDate = '2014-06-01'
	SET @EndDate = '2015-05-01'
	INSERT INTO @CustomerIDs (ID) VALUES (4)
	INSERT INTO @CustomerIDs (ID) VALUES (5)
	INSERT INTO @CustomerIDs (ID) VALUES (6)
	INSERT INTO @CustomerIDs (ID) VALUES (7)
	INSERT INTO @CustomerIDs (ID) VALUES (8)
	INSERT INTO @CustomerIDs (ID) VALUES (9)
	INSERT INTO @CustomerIDs (ID) VALUES (10)
	INSERT INTO @CustomerIDs (ID) VALUES (11)
	INSERT INTO @CustomerIDs (ID) VALUES (12)
	INSERT INTO @CustomerIDs (ID) VALUES (13)
	INSERT INTO @CustomerIDs (ID) VALUES (14)
	INSERT INTO @CustomerIDs (ID) VALUES (15)
	INSERT INTO @CustomerIDs (ID) VALUES (16)
	-- */ -- END TEST DATA

	SELECT
		cd.DataPeriod AS SalesFirstDayOfMonth,
		SUM(cd.DataSalesActual) AS SumOfSalesDollars,
		SUM(cd.DataSalesForecast) AS SumOfSalesForecastDollars,
		SUM(cd.DataCostActual) AS SumOfSalesCost,
		SUM(cd.DataCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerData AS cd
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cd.CustomerID
	WHERE cd.DataPeriod >= @StartDate
		AND cd.DataPeriod <= @EndDate
	GROUP BY cd.DataPeriod
	ORDER BY cd.DataPeriod

	RETURN
	
-- /* 
END
-- */


GO

