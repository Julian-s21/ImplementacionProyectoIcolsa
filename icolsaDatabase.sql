CREATE DATABASE IF NOT EXISTS icolsaDatabase;
USE icolsaDatabase;

-- ================================
--             TABLA USUARIO
-- ================================
CREATE TABLE Usuario (
    IDUsuario INT PRIMARY KEY AUTO_INCREMENT,
    Nombre_Usuario VARCHAR(200),
    Correo_Usuario VARCHAR(200),
    Contrasena_Usuario VARCHAR(200),
    Rol_Usuario VARCHAR(100)
);

-- ================================
--             TABLA CLIENTE
-- ================================
CREATE TABLE Cliente (
    IDCliente INT PRIMARY KEY AUTO_INCREMENT,
    Nombre_Cliente VARCHAR(200),
    Apellido_Cliente VARCHAR(200),
    Direccion_Cliente VARCHAR(300),
    Telefono_Cliente VARCHAR(50),
    Correo_Cliente VARCHAR(200),
    NIT_Cliente VARCHAR(50)
);

-- ================================
--             TABLA CATEGORIA
-- ================================
CREATE TABLE Categoria (
    IDCategoria INT PRIMARY KEY AUTO_INCREMENT,
    Nombre_Categoria VARCHAR(200)
);

-- ================================
--             TABLA PRODUCTO
-- ================================
CREATE TABLE Producto (
    IDProducto INT PRIMARY KEY AUTO_INCREMENT,
    Nombre_Producto VARCHAR(200),
    Codigo_Producto VARCHAR(100),
    PrecioUnitario_Producto DECIMAL(10,2),
    Stock_Producto INT,
    ImagenUrl VARCHAR(300),
    IDCategoria INT,

    CONSTRAINT FK_Producto_Categoria
        FOREIGN KEY (IDCategoria)
        REFERENCES Categoria(IDCategoria)
        ON DELETE RESTRICT
);

-- ================================
--             TABLA PEDIDO
-- ================================
CREATE TABLE Pedido (
    IDPedido INT PRIMARY KEY AUTO_INCREMENT,
    Fecha_Pedido DATETIME,
    Cantidad_Pedido INT,
    TotalPago_Pedido DECIMAL(10,2),

    IDCliente INT,
    IDProducto INT,
    IDUsuario INT,

    CONSTRAINT FK_Pedido_Cliente
        FOREIGN KEY (IDCliente)
        REFERENCES Cliente(IDCliente)
        ON DELETE RESTRICT,

    CONSTRAINT FK_Pedido_Producto
        FOREIGN KEY (IDProducto)
        REFERENCES Producto(IDProducto)
        ON DELETE RESTRICT,

    CONSTRAINT FK_Pedido_Usuario
        FOREIGN KEY (IDUsuario)
        REFERENCES Usuario(IDUsuario)
        ON DELETE RESTRICT
);

-- ================================
--         TABLA DETALLE PEDIDO
-- ================================
CREATE TABLE DetallePedido (
    IDDetallePedido INT PRIMARY KEY AUTO_INCREMENT,

    IDPedido INT,
    IDProducto INT,
    
    Cantidad INT,
    PrecioUnitario DECIMAL(10,2),
    Subtotal DECIMAL(10,2),

    CONSTRAINT FK_DetallePedido_Pedido
        FOREIGN KEY (IDPedido)
        REFERENCES Pedido(IDPedido)
        ON DELETE RESTRICT,

    CONSTRAINT FK_DetallePedido_Producto
        FOREIGN KEY (IDProducto)
        REFERENCES Producto(IDProducto)
        ON DELETE RESTRICT
);

-- ================================
--          TABLA METODO PAGO
-- ================================
CREATE TABLE MetodoPago (
    IDMetodoPago INT PRIMARY KEY AUTO_INCREMENT,
    NombreMetodo VARCHAR(50) NOT NULL
);

-- ================================
--           TABLA ESTADO PAGO
-- ================================
CREATE TABLE EstadoPago (
    IDEstadoPago INT PRIMARY KEY AUTO_INCREMENT,
    NombreEstado VARCHAR(50) NOT NULL
);

-- ================================
--               TABLA PAGO
-- ================================
CREATE TABLE Pago (
    IDPago INT PRIMARY KEY AUTO_INCREMENT,

    Fecha_Pago DATETIME NOT NULL,
    Monto_Pago DECIMAL(10,2) NOT NULL,

    IDMetodoPago INT NOT NULL,
    IDPedido INT NOT NULL,
    IDEstadoPago INT NULL,

    CONSTRAINT FK_Pago_Metodo
        FOREIGN KEY (IDMetodoPago)
        REFERENCES MetodoPago(IDMetodoPago)
        ON DELETE RESTRICT,

    CONSTRAINT FK_Pago_Pedido
        FOREIGN KEY (IDPedido)
        REFERENCES Pedido(IDPedido)
        ON DELETE CASCADE,

    CONSTRAINT FK_Pago_Estado
        FOREIGN KEY (IDEstadoPago)
        REFERENCES EstadoPago(IDEstadoPago)
        ON DELETE RESTRICT
);

-- ================================
--         TABLA HISTORIAL INVENTARIO
-- ================================
CREATE TABLE HistorialInventarioSaldo (
    IDInventarioSaldo INT PRIMARY KEY AUTO_INCREMENT,

    TipoMovimiento VARCHAR(100),
    Saldo_Actual INT,
    Fecha_Actualizacion DATETIME,

    IDProducto INT,

    CONSTRAINT FK_Inventario_Producto
        FOREIGN KEY (IDProducto)
        REFERENCES Producto(IDProducto)
        ON DELETE RESTRICT
);

-- ================================
--               TABLA REPORTE
-- ================================
CREATE TABLE Reporte (
    IDReporte INT PRIMARY KEY AUTO_INCREMENT,

    Tipo_Reporte VARCHAR(200),
    Fecha_Reporte DATETIME,

    IDUsuario INT,

    CONSTRAINT FK_Reporte_Usuario
        FOREIGN KEY (IDUsuario)
        REFERENCES Usuario(IDUsuario)
        ON DELETE RESTRICT
);
