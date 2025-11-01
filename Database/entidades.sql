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
