-- ========================================
-- SignalR Database - DROP Tables Script
-- ========================================
-- WARNING: This will delete all tables and data!
-- The database itself will remain

USE SignalRDB;
GO

PRINT 'Dropping tables from SignalRDB...';
PRINT '';

-- Drop Mensajes table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Mensajes')
BEGIN
    DROP TABLE Mensajes;
    PRINT '✓ Table Mensajes dropped';
END
ELSE
BEGIN
    PRINT '⚠️  Table Mensajes does not exist';
END

-- Drop Servicios table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Servicios')
BEGIN
    DROP TABLE Servicios;
    PRINT '✓ Table Servicios dropped';
END
ELSE
BEGIN
    PRINT '⚠️  Table Servicios does not exist';
END

-- Drop Usuarios table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    DROP TABLE Usuarios;
    PRINT '✓ Table Usuarios dropped';
END
ELSE
BEGIN
    PRINT '⚠️  Table Usuarios does not exist';
END

PRINT '';
PRINT '========================================';
PRINT '  ALL TABLES DELETED!';
PRINT '========================================';
PRINT '';
PRINT 'Database SignalRDB still exists (empty)';
PRINT 'To recreate tables, run: schema.sql';
PRINT '========================================';
GO