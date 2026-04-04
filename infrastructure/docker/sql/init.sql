-- ─────────────────────────────────────────────────────────────────────
-- init.sql — SQL Server Initialization Script
--
-- Runs ONCE when the SQL Server container starts for the FIRST time.
-- Creates all databases for all StaffPro microservices.
-- ─────────────────────────────────────────────────────────────────────

-- Wait 5 seconds for SQL Server to fully start up before running
-- (SQL Server needs a moment after the process starts before accepting connections)
WAITFOR DELAY '00:00:05';
GO

-- ── Company Service Database ─────────────────────────────────────────
-- IF NOT EXISTS: checks if the database already exists before trying to create it
-- This makes the script SAFE to run multiple times (idempotent)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Company')
BEGIN
    CREATE DATABASE StaffPro_Company
    COLLATE Latin1_General_CI_AS;   -- Case-Insensitive, Accent-Sensitive (standard for English)
    PRINT 'Database StaffPro_Company created.';
END
ELSE
BEGIN
    PRINT 'Database StaffPro_Company already exists.';
END
GO

-- ── Client Service Database ──────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Client')
BEGIN
    CREATE DATABASE StaffPro_Client
    COLLATE Latin1_General_CI_AS;
    PRINT 'Database StaffPro_Client created.';
END
GO

-- ── Employee Service Database ────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Employee')
BEGIN
    CREATE DATABASE StaffPro_Employee
    COLLATE Latin1_General_CI_AS;
    PRINT 'Database StaffPro_Employee created.';
END
GO

-- ── Jobs Service Database ────────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Jobs')
BEGIN
    CREATE DATABASE StaffPro_Jobs
    COLLATE Latin1_General_CI_AS;
    PRINT 'Database StaffPro_Jobs created.';
END
GO

-- ── Careers Service Database ─────────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Careers')
BEGIN
    CREATE DATABASE StaffPro_Careers
    COLLATE Latin1_General_CI_AS;
    PRINT 'Database StaffPro_Careers created.';
END
GO

-- ── Dashboard Service Database ───────────────────────────────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffPro_Dashboard')
BEGIN
    CREATE DATABASE StaffPro_Dashboard
    COLLATE Latin1_General_CI_AS;
    PRINT 'Database StaffPro_Dashboard created.';
END
GO

PRINT 'All StaffPro databases initialized successfully!';
GO