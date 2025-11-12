-- Creación de la base de datos
CREATE DATABASE IF NOT EXISTS icolsaDatabase
USE icolsaDatabase;

-- Tabla Usuario
CREATE TABLE Usuario (
    IDUsuario INT AUTO_INCREMENT PRIMARY KEY,
    Nombre_Usuario VARCHAR(100) NOT NULL,
    Correo_Usuario VARCHAR(150) NOT NULL UNIQUE,
    Contrasena_Usuario VARCHAR(200) NOT NULL,
    Rol_Usuario VARCHAR(50) NOT NULL
);

-- Tabla Cliente
CREATE TABLE Cliente (
    IDCliente INT AUTO_INCREMENT PRIMARY KEY,
    Nombre_ClientE VARCHAR(100) NOT NULL,
    Apellido_Cliente VARCHAR(100) NOT NULL,
    Direccion_Cliente VARCHAR(200),
    Telefono_Cliente VARCHAR(20),
    Correo_Cliente VARCHAR(150),
    NIT_Cliente VARCHAR(20)
);

-- Tabla Categoria
CREATE TABLE Categoria (
    IDCategoria INT AUTO_INCREMENT PRIMARY KEY,
    Nombre_Categoria VARCHAR(100) NOT NULL
);

-- Tabla StockProducto
CREATE TABLE StockProducto (
    IDStockProducto INT AUTO_INCREMENT PRIMARY KEY,
    Cantidad_Ingreso INT NOT NULL DEFAULT 0,
    Cantidad_Ventas INT NOT NULL DEFAULT 0
);

-- Tabla Producto
CREATE TABLE Producto (
    IDProducto INT AUTO_INCREMENT PRIMARY KEY,
    Nombre_Producto VARCHAR(100) NOT NULL,
    Codigo_Producto VARCHAR(50) UNIQUE,
    PrecioUnitario_Producto DECIMAL(10,2) NOT NULL,
    ImagenUrl VARCHAR(50) NOT NULL,
    IDCategoria INT,
    IDStockProducto INT,
    FOREIGN KEY (IDCategoria) REFERENCES Categoria(IDCategoria),
    FOREIGN KEY (IDStockProducto) REFERENCES StockProducto(IDStockProducto)
);
ALTER TABLE Producto
MODIFY COLUMN ImagenUrl VARCHAR(1000);



INSERT INTO Categorias (Nombre_Categoria)
VALUES
('Hilos'),
('Globos'),
('Accesorios'),
('Decoraciones');


INSERT INTO Productos (Nombre_Producto, Codigo_Producto, PrecioUnitario_Producto, ImagenUrl, IDCategoria, IDStockProducto)
VALUES
('Hilos Iris Caja Amarilla', 'HIL-IR-AM', 15.00,  'https://hilosiris.com/cdn/shop/files/CAJASEDAAMARILLA.png?v=1733323466', 1, 1),
('Hilos Iris Caja Roja', 'HIL-IR-RO', 15.00, 'https://i.imgur.com/RUOuF1P.jpg', 1, 2),
('Hilos Omega #18', 'HIL-OM-18', 18.00, 'https://i.imgur.com/jhc9uUB.jpg', 1, 3),
('Hilos Espiga #22', 'HIL-ES-22', 17.00, 'https://i.imgur.com/CfA6T7C.jpg', 1, 4),
('Hilos Zapatero', 'HIL-ZA-01', 16.00, 'https://i.imgur.com/aDkP6Zy.jpg', 1, 5),
('Hilos Lustrina', 'HIL-LU-01', 14.00, 'https://i.imgur.com/h6CmFwh.jpg', 1, 6),
('Hilos Tubo', 'HIL-TU-01', 12.00, 'https://i.imgur.com/rcw7pOv.jpg', 1, 7),
('Globos #9 Normal', 'GLO-09-NO', 25.00, 'https://i.imgur.com/YPlEPX8.jpg', 2, 8),
('Globo Decorado', 'GLO-DE-01', 30.00, 'https://i.imgur.com/V8cP6Cq.jpg', 2, 9),
('Globo Metálico', 'GLO-ME-01', 35.00, 'https://i.imgur.com/wFJtzVZ.jpg', 2, 10),
('Globo Largo #270', 'GLO-27-LA', 28.00, 'https://i.imgur.com/N9mSX4m.jpg', 2, 11);

UPDATE Producto
SET ImagenUrl = CASE Codigo_Producto
  WHEN 'HIL-IR-AM' THEN 'https://hilosiris.com/cdn/shop/files/CAJASEDAAMARILLA.png?v=1733323466'
  WHEN 'HIL-IR-RO' THEN 'https://hilosiris.com/cdn/shop/files/CAJASEDAROJA.png?v=1733323500'
  WHEN 'HIL-OM-18' THEN 'https://www.estambres.com/cdn/shop/products/omega18.jpg?v=1631720195'
  WHEN 'HIL-ES-22' THEN 'https://acdn-us.mitiendanube.com/stores/002/644/885/products/espiga22.png'
  WHEN 'HIL-ZA-01' THEN 'https://acdn-us.mitiendanube.com/stores/002/644/885/products/zapatero.png'
  WHEN 'HIL-LU-01' THEN 'https://hilosiris.com/cdn/shop/files/lustrina.png?v=1733323522'
  WHEN 'HIL-TU-01' THEN 'https://hilosiris.com/cdn/shop/files/hilotubo.png?v=1733323544'
  WHEN 'GLO-09-NO' THEN 'https://dismartgt.com/cdn/shop/products/globoslisossurtidosweb_Mesadetrabajo1.png?v=1642611303'
  WHEN 'GLO-DE-01' THEN 'https://fiestafiestagt.com/userfiles/2020/08/Surtido-removebg-preview.png'
  WHEN 'GLO-ME-01' THEN 'https://walmartgt.vtexassets.com/arquivos/ids/168121/Globo-l-tex-No-9-surtido-colores-50u-1-16813.jpg?v=637605490887030000'
  WHEN 'GLO-27-LA' THEN 'https://comercialzazueta.com/cdn/shop/products/Payaso-9-Violet.png?v=1592371847'
  ELSE ImagenUrl
END
WHERE Codigo_Producto IN (
  'HIL-IR-AM','HIL-IR-RO','HIL-OM-18','HIL-ES-22','HIL-ZA-01','HIL-LU-01','HIL-TU-01',
  'GLO-09-NO','GLO-DE-01','GLO-ME-01','GLO-27-LA'
);

SELECT Codigo_Producto, ImagenUrl FROM Producto;


UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/6vTt2sF.jpg' WHERE IDProducto = 1;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/WfEao1E.jpg' WHERE IDProducto = 2;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/nM0Z5XG.jpg' WHERE IDProducto = 3;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/Bcwq4QB.jpg' WHERE IDProducto = 4;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/fxK1qX9.jpg' WHERE IDProducto = 5;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/uwH2uQ3.jpg' WHERE IDProducto = 6;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/3dERk0N.jpg' WHERE IDProducto = 7;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/s6hXixW.jpg' WHERE IDProducto = 8;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/9jUz3qT.jpg' WHERE IDProducto = 9;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/Zv1MQsr.jpg' WHERE IDProducto = 10;
UPDATE Productos SET ImagenUrl = 'https://i.imgur.com/5ZJe5mB.jpg' WHERE IDProducto = 11;


INSERT INTO StockProductos (Cantidad_Ingreso, Cantidad_Ventas)
VALUES
(100, 20),
(120, 30),
(90, 15),
(80, 10),
(70, 12),
(60, 8),
(50, 5),
(200, 60),
(180, 40),
(150, 30),
(130, 25);
INSERT INTO Usuarios (Nombre_Usuario, Correo_Usuario, Contrasena_Usuario, Rol_Usuario)
VALUES 
('Julian', 'test2', '123', 'Usuario')

INSERT INTO Usuarios (Nombre_Usuario, Correo_Usuario, Contrasena_Usuario, Rol_Usuario)
VALUES 
('Julian', 'test', '123', 'Administrativo')

-- Tabla Pedido
CREATE TABLE Pedido (
    IDPedido INT AUTO_INCREMENT PRIMARY KEY,
    Fecha_Pedido DATE NOT NULL,
    Cantidad_Pedido INT NOT NULL,
    TotalPago_Pedido DECIMAL(10,2) NOT NULL,
    IDCliente INT,
    IDProducto INT,
    IDUsuario INT,
    FOREIGN KEY (IDCliente) REFERENCES Cliente(IDCliente),
    FOREIGN KEY (IDProducto) REFERENCES Producto(IDProducto),
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);

-- Tabla Pago
CREATE TABLE Pago (
    IDPago INT AUTO_INCREMENT PRIMARY KEY,
    Metodo_Pago VARCHAR(50) NOT NULL,
    Fecha_Pago DATE NOT NULL,
    Monto_Pago DECIMAL(10,2) NOT NULL,
    Estado_Pago VARCHAR(50) NOT NULL,
    IDCliente INT,
    FOREIGN KEY (IDCliente) REFERENCES Cliente(IDCliente)
);

-- Tabla Reportes
CREATE TABLE Reportes (
    IDReporte INT AUTO_INCREMENT PRIMARY KEY,
    Tipo_Reporte VARCHAR(100) NOT NULL,
    Fecha_Reporte DATE NOT NULL,
    IDUsuario INT,
    FOREIGN KEY (IDUsuario) REFERENCES Usuario(IDUsuario)
);

-- Tabla HistorialInventarioSaldo
CREATE TABLE HistorialInventarioSaldo (
    IDInventarioSaldo INT AUTO_INCREMENT PRIMARY KEY,
    TipoMovimiento VARCHAR(100) NOT NULL,
    Saldo_Actual INT NOT NULL,
    Fecha_Actualizacion DATE NOT NULL,
    IDProducto INT,
    FOREIGN KEY (IDProducto) REFERENCES Producto(IDProducto)
);
