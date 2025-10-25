-- ========================================
-- SignalR Database Schema
-- ========================================
-- Complete database setup with schema and sample data
-- Run this script to create the database from scratch

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SignalRDB')
BEGIN
    CREATE DATABASE SignalRDB;
    PRINT '✓ Database SignalRDB created';
END
ELSE
BEGIN
    PRINT '✓ Database SignalRDB already exists';
END
GO

USE SignalRDB;
GO

-- ========================================
-- SCHEMA
-- ========================================

-- Table: Usuarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL
    );
    PRINT '✓ Table Usuarios created';
END
GO

-- Table: Servicios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Servicios')
BEGIN
    CREATE TABLE Servicios (
        IdServicio INT PRIMARY KEY IDENTITY(1,1),
        Nombre NVARCHAR(50) NOT NULL
    );
    PRINT '✓ Table Servicios created';
END
GO

-- Table: Mensajes (for real-time chat)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Mensajes')
BEGIN
    CREATE TABLE Mensajes (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Texto NVARCHAR(500) NOT NULL,
        Fecha DATETIME NOT NULL,
        Usuario NVARCHAR(50) NULL
    );
    PRINT '✓ Table Mensajes created';
END
GO

-- ========================================
-- SAMPLE DATA
-- ========================================

-- Insert sample users
IF NOT EXISTS (SELECT * FROM Usuarios WHERE Nombre = 'Juan')
BEGIN
    INSERT INTO Usuarios (Nombre) VALUES 
        ('Juan'),
        ('María'),
        ('Pedro'),
        ('Ana'),
        ('Carlos'),
        ('Lucía');
    PRINT '✓ Sample users inserted';
END
GO

-- Insert sample services
IF NOT EXISTS (SELECT * FROM Servicios WHERE Nombre = 'Pizza Express')
BEGIN
    INSERT INTO Servicios (Nombre) VALUES 
        ('Pizza Express'),
        ('Sushi Tokyo'),
        ('Burger House'),
        ('Café Central'),
        ('Pasta Italiana'),
        ('Tacos El Güero');
    PRINT '✓ Sample services inserted';
END
GO

-- Insert sample chat messages
IF NOT EXISTS (SELECT * FROM Mensajes WHERE Usuario = 'Sistema')
BEGIN
    INSERT INTO Mensajes (Texto, Fecha, Usuario) VALUES 
        ('¡Bienvenidos al chat en tiempo real!', DATEADD(MINUTE, -10, GETDATE()), 'Sistema'),
        ('Hola a todos 👋', DATEADD(MINUTE, -8, GETDATE()), 'Juan'),
        ('¿Cómo están?', DATEADD(MINUTE, -7, GETDATE()), 'María'),
        ('Todo bien, gracias!', DATEADD(MINUTE, -6, GETDATE()), 'Pedro'),
        ('Este chat con SignalR está genial', DATEADD(MINUTE, -4, GETDATE()), 'Ana'),
        ('Sí, los mensajes aparecen al instante 🚀', DATEADD(MINUTE, -2, GETDATE()), 'Carlos');
    PRINT '✓ Sample messages inserted';
END
GO

-- ========================================
-- SUMMARY
-- ========================================

PRINT '';
PRINT '========================================';
PRINT '  DATABASE SETUP COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Tables created:';
PRINT '  • Usuarios      (' + CAST((SELECT COUNT(*) FROM Usuarios) AS VARCHAR) + ' records)';
PRINT '  • Servicios     (' + CAST((SELECT COUNT(*) FROM Servicios) AS VARCHAR) + ' records)';
PRINT '  • Mensajes      (' + CAST((SELECT COUNT(*) FROM Mensajes) AS VARCHAR) + ' records)';
PRINT '';
PRINT 'Ready to use! 🎉';
PRINT '========================================';
GO
