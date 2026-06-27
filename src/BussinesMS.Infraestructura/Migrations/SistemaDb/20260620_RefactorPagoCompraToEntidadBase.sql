-- ============================================================
-- Migración 2: RefactorPagoCompraToEntidadBase
-- Convierte PagoCompra para heredar de EntidadBase
-- Agrega columnas de auditoría y PagadoPorUsuarioId
-- Elimina RegistradoByUsuarioId obsoleto
-- ============================================================

-- Agregar columnas de auditoría (EntidadBase)
ALTER TABLE PagosCompra ADD IsActive BIT NOT NULL DEFAULT 1;
ALTER TABLE PagosCompra ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
ALTER TABLE PagosCompra ADD UpdatedAt DATETIME2 NULL;
ALTER TABLE PagosCompra ADD DeletedAt DATETIME2 NULL;
ALTER TABLE PagosCompra ADD CreatedByUsuarioId INT NOT NULL DEFAULT 1;
ALTER TABLE PagosCompra ADD UpdatedByUsuarioId INT NULL;
ALTER TABLE PagosCompra ADD DeletedByUsuarioId INT NULL;

-- Agregar PagadoPorUsuarioId (quien físicamente pagó)
ALTER TABLE PagosCompra ADD PagadoPorUsuarioId INT NOT NULL DEFAULT 1;

-- Migrar datos existentes: copiar RegistradoByUsuarioId -> PagadoPorUsuarioId
UPDATE PagosCompra SET PagadoPorUsuarioId = RegistradoByUsuarioId WHERE PagadoPorUsuarioId = 1;

-- Eliminar columna antigua
ALTER TABLE PagosCompra DROP COLUMN RegistradoByUsuarioId;
