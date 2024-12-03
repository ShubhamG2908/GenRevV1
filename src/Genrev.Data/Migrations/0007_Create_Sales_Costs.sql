﻿-- =========================================
-- Replace GPP Columns with Cost columns
-- =========================================
ALTER TABLE dbo.CustomerSales ADD SalesCost DECIMAL(19,4);
ALTER TABLE dbo.CustomerSales ADD SalesCostForecast DECIMAL(19,4);
GO

UPDATE dbo.CustomerSales SET SalesCost = SalesDollars - (SalesDollars * SalesGPP / 100);
UPDATE dbo.CustomerSales SET SalesCostForecast = SalesForecastDollars - (SalesForecastDollars * SalesForecastGPP / 100);
GO

ALTER TABLE dbo.CustomerSales DROP COLUMN SalesGPP;
ALTER TABLE dbo.CustomerSales DROP COLUMN SalesForecastGPP;
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
		cs.SalesFirstDayOfMonth,
		SUM(cs.SalesDollars) AS SumOfSalesDollars,
		SUM(cs.SalesForecastDollars) AS SumOfSalesForecastDollars,
		SUM(cs.SalesCost) AS SumOfSalesCost,
		SUM(cs.SalesCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerSales AS cs
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cs.CustomerID
	WHERE cs.SalesFirstDayOfMonth >= @StartDate
		AND cs.SalesFirstDayOfMonth <= @EndDate
	GROUP BY cs.SalesFirstDayOfMonth
	ORDER BY cs.SalesFirstDayOfMonth

	RETURN
	
-- /* 
END
-- */

GO










-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by industry
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
		SUM(cs.SalesDollars) AS SumOfSalesDollars,
		SUM(cs.SalesForecastDollars) AS SumOfSalesForecastDollars,
		SUM(cs.SalesCost) AS SumOfSalesCost,
		SUM(cs.SalesCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerSales AS cs
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cs.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cs.CustomerID = cc.ID
	INNER JOIN dbo.CompanyIndustries AS ci ON ci.ID = cc.CustomerIndustryID
	WHERE cs.SalesFirstDayOfMonth >= @StartDate
		AND cs.SalesFirstDayOfMonth <= @EndDate
	GROUP BY cc.CustomerIndustryID, ci.IndustryName
	ORDER BY ci.IndustryName
	RETURN
	
-- /* 
END
-- */

GO








-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by customer
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
		SUM(cs.SalesDollars) AS SumOfSalesDollars,
		SUM(cs.SalesForecastDollars) AS SumOfSalesForecastDollars,
		SUM(cs.SalesCost) AS SumOfSalesCost,
		SUM(cs.SalesCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerSales AS cs
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cs.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cs.CustomerID = cc.ID
	INNER JOIN dbo.CompanyCustomerTypes AS ct ON ct.ID = cc.CustomerTypeID
	WHERE cs.SalesFirstDayOfMonth >= @StartDate
		AND cs.SalesFirstDayOfMonth <= @EndDate
	GROUP BY cc.CustomerTypeID, ct.TypeName
	ORDER BY ct.TypeName
	RETURN
	
-- /* 
END
-- */

GO







-- =========================================
-- Retrieve basic customer sales and forecast info aggregates by customer
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
		cs.CustomerID,
		cc.CustomerName,
		SUM(cs.SalesDollars) AS SumOfSalesDollars,
		SUM(cs.SalesForecastDollars) AS SumOfSalesForecastDollars,
		SUM(cs.SalesCost) AS SumOfSalesCost,
		SUM(cs.SalesCostForecast) AS SumOfSalesCostForecast		
	FROM dbo.CustomerSales AS cs
	INNER JOIN @CustomerIDs AS cids ON cids.ID = cs.CustomerID
	INNER JOIN dbo.CompanyCustomers AS cc ON cs.CustomerID = cc.ID
	WHERE cs.SalesFirstDayOfMonth >= @StartDate
		AND cs.SalesFirstDayOfMonth <= @EndDate
	GROUP BY cs.CustomerID, cc.CustomerName
	ORDER BY cc.CustomerName
	RETURN
	
-- /* 
END
-- */

GO




