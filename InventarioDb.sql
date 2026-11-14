/* =========================================================
   Script de base de datos - InventarioApi
   Motor: SQL Server
   Reconstruye la base InventarioDb con sus tablas,
   relación 1:N (Categorias -> Productos) y restricciones.
   ========================================================= */

IF DB_ID('InventarioDb') IS NULL
    CREATE DATABASE InventarioDb;
GO

USE InventarioDb;
GO

/* ---------- Tabla Categorias ---------- */
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID('dbo.Categorias', 'U') IS NOT NULL DROP TABLE dbo.Categorias;
GO

CREATE TABLE dbo.Categorias (
    Id          INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(50)      NOT NULL,
    Descripcion NVARCHAR(200)     NULL,
    CONSTRAINT PK_Categorias PRIMARY KEY (Id)
);
GO

-- Regla 2: no se permiten categorías con el mismo nombre
CREATE UNIQUE INDEX IX_Categorias_Nombre ON dbo.Categorias (Nombre);
GO

/* ---------- Tabla Productos ---------- */
CREATE TABLE dbo.Productos (
    Id            INT IDENTITY(1,1) NOT NULL,
    Nombre        NVARCHAR(50)      NOT NULL,
    Descripcion   NVARCHAR(200)     NULL,
    Precio        DECIMAL(18,2)     NOT NULL,
    Stock         INT               NOT NULL,
    FechaRegistro DATETIME2         NOT NULL CONSTRAINT DF_Productos_FechaRegistro DEFAULT (GETDATE()),
    CategoriaId   INT               NOT NULL,
    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    -- Relación 1:N: cada producto pertenece a una categoría.
    -- NO ACTION (Restrict): impide eliminar una categoría con productos asociados (Regla 4)
    CONSTRAINT FK_Productos_Categorias_CategoriaId
        FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias (Id) ON DELETE NO ACTION,
    CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),   -- Precio mayor que cero
    CONSTRAINT CK_Productos_Stock  CHECK (Stock >= 0)    -- Regla 5: stock no negativo
);
GO

CREATE INDEX IX_Productos_CategoriaId ON dbo.Productos (CategoriaId);
-- Regla 1: no se permiten productos con el mismo nombre
-- (la intercalación por defecto de SQL Server ya ignora mayúsculas/minúsculas)
CREATE UNIQUE INDEX IX_Productos_Nombre ON dbo.Productos (Nombre);
GO

/* ---------- Datos de ejemplo (opcional) ---------- */
INSERT INTO dbo.Categorias (Nombre, Descripcion) VALUES
    (N'Electrónica', N'Dispositivos y accesorios electrónicos'),
    (N'Librería',    N'Útiles escolares y de oficina');

INSERT INTO dbo.Productos (Nombre, Descripcion, Precio, Stock, CategoriaId) VALUES
    (N'Teclado USB', N'Teclado en español', 120000, 15, 1),
    (N'Cuaderno A4', N'100 hojas rayadas',   15000, 50, 2);
GO
