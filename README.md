
# ProductsMvc

A minimal ASP.NET Core MVC app that lists products from SQL Server.

## Prerequisites
- Visual Studio 2022 (or later) with .NET SDK (net8.0)
- Access to SQL Server `YOUR_SQL_SERVER ` and database `ProductsDb`

## Configure & Run
1. Open `ProductsMvc.csproj` in Visual Studio.
2. Restore NuGet packages.
3. Ensure `appsettings.json` connection string is correct.
4. Press **F5** to run. Default route goes to `Products/Index`.

## SQL Table
Use this script to create the table if it does not exist:
```sql
USE [ProductsDb];
IF OBJECT_ID('dbo.Products','U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(200) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL
    );

    INSERT INTO dbo.Products (Name, Price, Stock) VALUES
    ('Widget A', 99.99, 10),
    ('Widget B', 149.50, 5),
    ('Widget C', 49.00, 25);
END
```
