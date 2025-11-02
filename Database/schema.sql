-- ========================================
-- SignalR Database Schema
-- ========================================
-- Complete database setup with schema and sample data
-- Run this script to create the database from scratch

USE master;

GO
-- Agregar Latitud y Longitud a Usuarios si no existen
IF COL_LENGTH('Usuarios', 'Latitud') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Latitud FLOAT NULL;
    PRINT '✓ Columna Latitud agregada a Usuarios';
END

IF COL_LENGTH('Usuarios', 'Longitud') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Longitud FLOAT NULL;
    PRINT '✓ Columna Longitud agregada a Usuarios';
END

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
        Nombre NVARCHAR(50) NOT NULL,
        Latitud FLOAT NULL,
        Longitud FLOAT NULL
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
    INSERT INTO Usuarios (Nombre, Latitud, Longitud) VALUES 
        ('Juan', -34.6037, -58.3816),
        ('María', -34.6090, -58.3772),
        ('Pedro', -34.6158, -58.4333),
        ('Ana', -34.6189, -58.3928),
        ('Carlos', -34.6115, -58.4468),
        ('Lucía', -34.6072, -58.3925);
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

DECLARE @UsuariosCount INT, @ServiciosCount INT, @MensajesCount INT;

SELECT @UsuariosCount = COUNT(*) FROM Usuarios;
SELECT @ServiciosCount = COUNT(*) FROM Servicios;
SELECT @MensajesCount = COUNT(*) FROM Mensajes;

PRINT '';
PRINT '========================================';
PRINT '  DATABASE SETUP COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Tables created:';
PRINT '  • Usuarios      (' + CAST(@UsuariosCount AS VARCHAR) + ' records)';
PRINT '  • Servicios     (' + CAST(@ServiciosCount AS VARCHAR) + ' records)';
PRINT '  • Mensajes      (' + CAST(@MensajesCount AS VARCHAR) + ' records)';
PRINT '';
PRINT 'Ready to use! 🎉';
PRINT '========================================';

-- =============================
-- CREACIÓN DE BASE DE DATOS
-- =============================

USE SignalRDB;
GO

-- =============================
-- TABLA: Comercio
-- =============================
CREATE TABLE Comercio (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(150),
    latitud DECIMAL(9,6) NOT NULL,
    longitud DECIMAL(9,6) NOT NULL
);
GO

-- =============================
-- TABLA: Producto
-- =============================
CREATE TABLE Producto (
    id INT IDENTITY(1,1) PRIMARY KEY,
    comercio_id INT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    categoria VARCHAR(50),
    precio DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (comercio_id) REFERENCES Comercio(id)
);
GO

-- =============================
-- TABLA: Repartidor
-- =============================
CREATE TABLE Repartidor (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    latitud_actual DECIMAL(9,6) NULL,
    longitud_actual DECIMAL(9,6) NULL,
    estado VARCHAR(20) DEFAULT 'disponible'
);
GO

-- =============================
-- TABLA: UsuarioFinal
-- =============================
CREATE TABLE UsuarioFinal (
    id INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    direccion VARCHAR(150) NOT NULL,
    latitud DECIMAL(9,6) NULL,
    longitud DECIMAL(9,6) NULL
);
GO

-- =============================
-- TABLA: Pedido
-- =============================
CREATE TABLE Pedido (
    id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NOT NULL,
    comercio_id INT NOT NULL,
    repartidor_id INT NULL,
    fecha_pedido DATETIME DEFAULT GETDATE(),
    estado VARCHAR(20) DEFAULT 'pendiente',
    total DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (usuario_id) REFERENCES UsuarioFinal(id),
    FOREIGN KEY (comercio_id) REFERENCES Comercio(id),
    FOREIGN KEY (repartidor_id) REFERENCES Repartidor(id)
);
GO

-- =============================
-- TABLA: PedidoDetalle
-- =============================
CREATE TABLE PedidoDetalle (
    id INT IDENTITY(1,1) PRIMARY KEY,
    pedido_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (pedido_id) REFERENCES Pedido(id),
    FOREIGN KEY (producto_id) REFERENCES Producto(id)
);
GO

-- =============================
-- DATOS DE EJEMPLO
-- =============================

-- Comercios
INSERT INTO Comercio (nombre, direccion, latitud, longitud)
VALUES 
('Pizzería Napoli', 'Av. Rivadavia 1234', -34.615803, -58.433297),
('BurgerZone', 'Av. Mitre 2300', -34.608523, -58.373953);

-- Productos
INSERT INTO Producto (comercio_id, nombre, categoria, precio)
VALUES
(1, 'Pizza Muzzarella', 'Pizzas', 3500),
(1, 'Pizza Napolitana', 'Pizzas', 4200),
(1, 'Coca Cola 1L', 'Bebidas', 1500),
(2, 'Burger Clásica', 'Hamburguesas', 2800),
(2, 'Papas Fritas', 'Acompañamientos', 1200);

-- Repartidores
INSERT INTO Repartidor (nombre, estado)
VALUES
('Juan Pérez', 'disponible'),
('María López', 'disponible');

-- Usuarios finales
INSERT INTO UsuarioFinal (nombre, direccion, latitud, longitud)
VALUES
('Facundo Palleros', 'Morón 1500', -34.654201, -58.619502),
('Ana Torres', 'Ituzaingó 350', -34.653480, -58.668710);

-- Pedido de ejemplo
INSERT INTO Pedido (usuario_id, comercio_id, repartidor_id, estado, total)
VALUES (1, 1, 1, 'en_camino', 5000);

INSERT INTO PedidoDetalle (pedido_id, producto_id, cantidad, precio_unitario)
VALUES 
(1, 1, 1, 3500),
(1, 3, 1, 1500);
GO

ALTER TABLE Pedido
ADD producto_id INT NULL;

ALTER TABLE Pedido
ADD CONSTRAINT FK_Pedido_Producto
FOREIGN KEY (producto_id) REFERENCES Producto(id);

SELECT *
FROM Pedido
GO

-- =============================
-- PROMIEDOS TABLES
-- =============================

-- =============================
-- TABLA: Partidos
-- =============================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Partidos')
BEGIN
    CREATE TABLE Partidos (
        Id INT PRIMARY KEY IDENTITY(1,1),
        EquipoLocal NVARCHAR(100) NOT NULL,
        EquipoVisitante NVARCHAR(100) NOT NULL,
        AmarillasEquipoLocal INT DEFAULT 0,
        AmarillasEquipoVisitante INT DEFAULT 0,
        RojasEquipoLocal INT DEFAULT 0,
        RojasEquipoVisitante INT DEFAULT 0,
        HorarioPartido DATETIME NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE()
    );
    PRINT '✓ Table Partidos created';
END
GO

-- =============================
-- TABLA: Goles
-- =============================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Goles')
BEGIN
    CREATE TABLE Goles (
        Id INT PRIMARY KEY IDENTITY(1,1),
        PartidoId INT NOT NULL,
        Minuto INT NOT NULL,
        Jugador NVARCHAR(100) NOT NULL,
        Equipo NVARCHAR(20) NOT NULL, -- 'local' o 'visitante'
        FechaGol DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (PartidoId) REFERENCES Partidos(Id) ON DELETE CASCADE,
        CONSTRAINT CK_Equipo CHECK (Equipo IN ('local', 'visitante'))
    );
    PRINT '✓ Table Goles created';
END
GO

-- =============================
-- DATOS DE EJEMPLO - PARTIDOS
-- =============================
IF NOT EXISTS (SELECT * FROM Partidos WHERE EquipoLocal = 'San Lorenzo')
BEGIN
    INSERT INTO Partidos (EquipoLocal, EquipoVisitante, AmarillasEquipoLocal, AmarillasEquipoVisitante, RojasEquipoLocal, RojasEquipoVisitante, HorarioPartido)
    VALUES 
        ('San Lorenzo', 'Riestra', 3, 2, 0, 0, DATEADD(DAY, -1, GETDATE())),
        ('Godoy Cruz', 'San Martín (SJ)', 1, 4, 0, 1, DATEADD(DAY, -1, GETDATE())),
        ('Newell''s', 'Unión', 2, 2, 0, 0, DATEADD(DAY, -1, GETDATE())),
        ('Aldosivi', 'Independiente', 3, 3, 1, 0, DATEADD(DAY, -1, GETDATE())),
        ('Boca Juniors', 'River Plate', 4, 3, 0, 0, DATEADD(DAY, -1, GETDATE()));
    PRINT '✓ Sample partidos inserted';
END
GO

PRINT '';
PRINT '========================================';
PRINT '  PROMIEDOS TABLES READY!';
PRINT '========================================';
PRINT '  • Partidos table created';
PRINT '  • Goles table created';
PRINT '  • Sample data inserted';
PRINT '========================================';