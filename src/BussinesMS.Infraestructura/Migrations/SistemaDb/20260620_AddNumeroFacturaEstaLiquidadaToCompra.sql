-- ============================================================
-- Migración 1: AddNumeroFacturaEstaLiquidadaToCompra
-- Agrega campos NumeroFactura y EstaLiquidada a la tabla Compras
-- ============================================================

-- NumeroFactura: número de factura del proveedor (nullable)
ALTER TABLE Compras ADD NumeroFactura NVARCHAR(100) NULL;

-- EstaLiquidada: indica si la compra fue pagada completamente
ALTER TABLE Compras ADD EstaLiquidada BIT NOT NULL DEFAULT 0;
