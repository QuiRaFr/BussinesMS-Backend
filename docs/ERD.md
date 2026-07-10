# BussinesMS - Entity Relationship Diagram

> Este documento define el modelo de datos completo del sistema.
> Se usa como referencia para crear nuevos módulos.
> Última actualización: 31/05/2026 — Refactorización hasta ProductoVariante.

---

## DB_AUTH — Base de datos de autenticación

**Propósito**: Usuarios, Roles, Almacenes. Compartida por DB_Regular y DB_Navidad. Los IDs de esta DB viajan en el JWT token.

### Tablas

| Tabla | Descripción |
|-------|-------------|
| Sistema | 1: Regular, 2: Navideño |
| Rol | Admin, VendedorTienda, VendedorRuta, EncargadoAlmacen, Contador |
| Usuario | Usuarios del sistema con JWT |
| Almacen | 3 almacenes (Tienda Principal, Almacén 1, Almacén 2) |

### Esquema

```
Table Sistema {
  Id int [pk, increment]
  Nombre nvarchar(50) [not null, note: '1: Regular, 2: Navideño']
  IsActive bit [not null, default: true]
}

Table Rol {
  Id int [pk, increment]
  Nombre nvarchar(50) [not null, unique,
    note: 'Admin, VendedorTienda, VendedorRuta, EncargadoAlmacen, Contador']
  Permisos nvarchar(max) [null, note: 'JSON array de permisos: ["venta.crear","caja.abrir"]']
  IsActive bit [not null, default: true]
}

Table Usuario {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null]
  Apellido nvarchar(100) [not null]
  Email nvarchar(150) [null]
  Username nvarchar(50) [not null, unique]
  PasswordHash nvarchar(255) [not null]
  RolId int [not null]
  SistemaIdDefault int [not null, default: 1]
  IsActive bit [not null, default: true]
}

Table Almacen {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null]
  Codigo nvarchar(20) [not null, unique]
  EsTienda bit [not null, default: false,
    note: 'true = Tienda Principal, false = Deposito']
  Direccion nvarchar(255) [null]
  IsActive bit [not null, default: true]
}

Ref: Usuario.RolId > Rol.Id
```

---

## DB_SISTEMA — Sistema principal (todo el año)

**Propósito**: Catálogo de productos, inventario, compras, ventas y caja.

> **Nota importante**: Los campos UsuarioId y AlmacenId son int simples. NO tienen FK hacia DB_Auth (vienen del JWT).

---

### MÓDULO 1: CATÁLOGO DE PRODUCTOS

#### Tablas

| Tabla | Estado | Descripción |
|-------|--------|-------------|
| Categoria | ✅ | Categorías de productos |
| Fabricante | ✅ | Fabricantes de productos |
| DescripcionSabor | ✅ | Sabores disponibles |
| DescripcionTamanio | ✅ | Tamaños (100g, 500ml, 1kg) |
| TipoPresentacion | ✅ | Unidad, Caja, Caja2 |
| Producto | ✅ | Producto maestro |
| ProductoVariante | ✅ | Variante (sabor + tamaño) |
| ProductoPresentacion | ✅ | Equivalencias por presentación (jerárquico) |
| HistorialPrecio | ⏳ | Histórico de cambios de precio |

#### Esquema

```
Table Categoria {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null]
  Descripcion nvarchar(255) [null]
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table Fabricante {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null]
  Descripcion nvarchar(255) [null]
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table DescripcionSabor {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null, unique]
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table DescripcionTamanio {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null, unique, note: 'Ej: 100g, 500ml, 1kg']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table TipoPresentacion {
  Id int [pk, increment]
  Nombre nvarchar(50) [not null, unique, note: 'Unidad, Caja, Caja2']
  Orden int [not null, note: 'Para mostrar de menor a mayor: 1=Unidad, 2=Caja, 3=Caja2']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table Producto {
  Id int [pk, increment]
  CodigoInterno nvarchar(20) [null, unique, note: 'Generado: PROD-00001']
  Nombre nvarchar(150) [not null]
  CategoriaId int [not null]
  FabricanteId int [null, note: 'Opcional']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}

Table ProductoVariante {
  Id int [pk, increment]
  ProductoId int [not null]
  NombreProducto nvarchar(150) [null, note: 'Copia del nombre del producto al momento de crear la variante']
  CodigoBarras nvarchar(50) [null]
  SaborId int [not null]
  SaborDescripcion nvarchar(100) [null, note: 'Copia del nombre del sabor']
  TamanioId int [not null]
  PesoTamanio nvarchar(50) [null, note: 'Copia del nombre del tamaño']
  PrecioVentaActual decimal(18,2) [not null, default: 0,
    note: 'Cache del precio vigente. La historia está en HistorialPrecio']
  PrecioCompra decimal(18,2) [not null, default: 0,
    note: 'Último precio de compra registrado']
  CodigoAlmacen nvarchar(20) [null, note: 'Código del almacén al que pertenece']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]

  indexes {
    (ProductoId, SaborId, TamanioId) [unique, name: 'UQ_Variante_Combinacion']
  }
}

Table ProductoPresentacion {
  Id int [pk, increment]
  VarianteId int [not null]
  TipoPresentacionId int [not null]
  NombrePersonalizado nvarchar(50) [null,
    note: 'Nombre custom: "Display","Tira","Botella". null = usa nombre del TipoPresentacion']
  CantidadDePadre int [not null, default: 1,
    note: 'Cuántas unidades del padre contiene. Unidad=1, Caja=12, etc.']
  PresentacionPadreId int [null,
    note: 'null = raíz (Unidad). FK self-ref a la presentación padre']
  EsDefaultReporte bit [not null, default: false,
    note: 'Solo 1 true por VarianteId. Presentación usada en reportes']
  CodigoBarras nvarchar(50) [null, unique,
    note: 'Código de barras específico de esta presentación']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]

  indexes {
    (VarianteId, TipoPresentacionId) [unique, name: 'UQ_Presentacion_Variante']
    (VarianteId) [unique, where: 'EsDefaultReporte = 1', name: 'UQ_Presentacion_DefaultReporte']
  }
}

Table HistorialPrecio {
  Id int [pk, increment]
  VarianteId int [not null]
  PrecioAnterior decimal(18,2) [not null]
  PrecioNuevo decimal(18,2) [not null]
  FechaCambio datetime2 [not null, default: `GETDATE()`]
  MotivoCambio nvarchar(255) [null]
  CambiadoByUsuarioId int [not null, note: 'ID del JWT, sin FK']
}

Ref: Producto.CategoriaId > Categoria.Id
Ref: Producto.FabricanteId > Fabricante.Id
Ref: ProductoVariante.ProductoId > Producto.Id
Ref: ProductoVariante.SaborId > DescripcionSabor.Id
Ref: ProductoVariante.TamanioId > DescripcionTamanio.Id
Ref: ProductoPresentacion.VarianteId > ProductoVariante.Id
Ref: ProductoPresentacion.TipoPresentacionId > TipoPresentacion.Id
Ref: ProductoPresentacion.PresentacionPadreId > ProductoPresentacion.Id
Ref: HistorialPrecio.VarianteId > ProductoVariante.Id
```

---

### MÓDULO 2: PROVEEDORES

#### Tablas

| Tabla | Estado | Descripción |
|-------|--------|-------------|
| Proveedor | ✅ | Proveedores del sistema |

#### Esquema

```
Table Proveedor {
  Id int [pk, increment]
  Nombre nvarchar(150) [not null]
  Nit nvarchar(20) [null]
  Telefono nvarchar(20) [null]
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
  DeletedAt datetime2 [null]
  CreatedByUsuarioId int [not null, note: 'ID del JWT, sin FK']
  UpdatedByUsuarioId int [null]
  DeletedByUsuarioId int [null]
}
```

---

### MÓDULO 3: INVENTARIO Y LOTES

#### Tablas

| Tabla | Estado | Descripción |
|-------|--------|-------------|
| InventarioLote | ⏳ | Lotes por fecha de vencimiento (FIFO) |
| MovimientoInventario | ⏳ | Auditoría de movimientos de stock |
| Traslado | ⏳ | Traslados entre almacenes |

#### Esquema

```
Table InventarioLote {
  Id int [pk, increment]
  VarianteId int [not null]
  AlmacenId int [not null]
  CompraDetalleId int [null, note: 'null = lote nacido de traslado parcial. Si viene de compra, referencia el detalle (de ahí se obtiene ProveedorId vía join)']

  StockInicial int [not null, note: 'Cantidad recibida originalmente, NUNCA cambia']
  StockDisponible int [not null, note: 'Disminuye al vender, trasladar (parcial) o marcar vencido']
  CantidadVencida int [not null, default: 0, note: 'Lo que venció sin venderse']

  CostoCompraUnitario decimal(18,4) [not null]
  PrecioVentaUnitario decimal(18,4) [not null]
  PrecioVentaMayoreo decimal(18,4) [not null]

  FechaVencimiento date [null, note: 'null = no vence']
  EstadoLote int [not null, default: 1, note: '1:Activo, 2:Agotado, 3:Vencido']

  IsActive bit [not null, default: true]
  CreatedAt datetime2
  UpdatedAt datetime2
  CreatedByUsuarioId int

  indexes {
    (VarianteId, AlmacenId, FechaVencimiento) [name: 'IX_Lote_FEFO']
  }
}

// Auditoría inmutable de cada cambio de stock
Table MovimientoInventario {
  Id int [pk, increment]
  LoteId int [not null]
  VarianteId int [not null]
  AlmacenOrigenId int [null, note: 'null si es entrada pura (compra)']
  AlmacenDestinoId int [null, note: 'null si es salida pura (venta)']
  TipoMovimiento int [not null,
    note: '1:EntradaCompra, 2:SalidaVenta, 3:Traslado, 4:AjustePositivo, 5:AjusteNegativo, 6:Vencimiento']
  CantidadUnidades int [not null, note: 'Siempre positivo. El tipo indica si suma o resta']
  SaldoResultante int [not null, note: 'StockDisponible del lote DESPUÉS de este movimiento']
  ReferenciaId int [null,
    note: 'ID del documento origen: VentaDetalleId, CompraDetalleId, TrasladoId, etc.']
  Observacion nvarchar(255) [null]
  FechaMovimiento datetime2 [not null, default: `GETDATE()`]
  UsuarioId int [not null, note: 'ID del JWT, sin FK']
}

// Traslados entre almacenes (TOTAL = mismo lote cambia de almacén; PARCIAL = se crea lote nuevo en destino)
Table Traslado {
  Id int [pk, increment]
  LoteId int [not null, note: 'Lote ORIGEN']
  LoteDestinoId int [null, note: 'null si fue TOTAL (mismo lote cambió de almacén). Con valor si fue PARCIAL (lote nuevo creado en destino)']
  AlmacenOrigenId int [not null, note: 'ID del JWT/DB_Auth, sin FK']
  AlmacenDestinoId int [not null, note: 'ID del JWT/DB_Auth, sin FK']
  CantidadUnidades int [not null]
  Observacion nvarchar(255) [null]
  FechaTraslado datetime2 [not null, default: `GETDATE()`]
  UsuarioId int [not null, note: 'ID del JWT, sin FK']
}

Ref: InventarioLote.VarianteId > ProductoVariante.Id
Ref: InventarioLote.ProveedorId > Proveedor.Id
Ref: InventarioLote.CompraDetalleId > CompraDetalle.Id
Ref: MovimientoInventario.LoteId > InventarioLote.Id
Ref: MovimientoInventario.VarianteId > ProductoVariante.Id
Ref: Traslado.VarianteId > ProductoVariante.Id
Ref: Traslado.LoteId > InventarioLote.Id
```

---

### MÓDULO 4: COMPRAS

#### Tablas

| Tabla | Estado | Descripción |
|-------|--------|-------------|
| Compra | ⏳ | Orden de compra |
| CompraDetalle | ⏳ | Detalle de productos comprados |
| PagoCompra | ⏳ | Pagos parciales a proveedores |

#### Esquema

```
Table Compra {
  Id int [pk, increment]
  ProveedorId int [not null]
  UsuarioId int [not null, note: 'ID del JWT, sin FK']
  AlmacenId int [not null, note: 'Almacén destino principal, ID del JWT, sin FK']
  FechaCompra datetime2 [not null, default: `GETDATE()`]
  TotalCompra decimal(18,2) [not null]
  EstadoPago int [not null, default: 1,
    note: '1:Pagado, 2:Credito, 3:ParcialmentePagado']
  Observacion nvarchar(500) [null]
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
}

Table CompraDetalle {
  Id int [pk, increment]
  CompraId int [not null]
  VarianteId int [not null]
  AlmacenId int [not null,
    note: 'Almacén específico de esta línea (puede diferir de Compra.AlmacenId)']
  CantidadUnidades int [not null]
  CostoUnitario decimal(18,4) [not null]
  FechaVencimiento date [null]
  LoteId int [null,
    note: 'FK al InventarioLote creado/actualizado. Se asigna al confirmar la compra']
}

Table PagoCompra {
  Id int [pk, increment]
  CompraId int [not null]
  Monto decimal(18,2) [not null]
  FechaPago datetime2 [not null, default: `GETDATE()`]
  SesionCajaId int [null,
    note: 'Si el pago salió de una caja activa, se registra aquí']
  Observacion nvarchar(255) [null]
  RegistradoByUsuarioId int [not null, note: 'ID del JWT, sin FK']
}

Ref: Compra.ProveedorId > Proveedor.Id
Ref: CompraDetalle.CompraId > Compra.Id
Ref: CompraDetalle.VarianteId > ProductoVariante.Id
Ref: CompraDetalle.LoteId > InventarioLote.Id
Ref: PagoCompra.CompraId > Compra.Id
Ref: PagoCompra.SesionCajaId > SesionCaja.Id
```

---

### MÓDULO 5: VENTAS Y CAJA

#### Tablas

| Tabla | Estado | Descripción |
|-------|--------|-------------|
| SesionCaja | ⏳ | Sesión de caja por usuario |
| Venta | ⏳ | Venta en tienda |
| VentaDetalle | ⏳ | Detalle con lote específico (FIFO) |
| CategoriaGasto | ⏳ | Categorías de gasto |
| GastoOperativo | ⏳ | Gastos operativos y pagos a proveedores |

#### Esquema

```
Table SesionCaja {
  Id int [pk, increment]
  UsuarioId int [not null, note: 'ID del JWT, sin FK']
  AlmacenId int [not null, note: 'ID del JWT/DB_Auth, sin FK']
  FechaApertura datetime2 [not null, default: `GETDATE()`]
  FechaCierre datetime2 [null]
  MontoInicial decimal(18,2) [not null, default: 0]
  IngresosEfectivo decimal(18,2) [not null, default: 0,
    note: 'Suma de ventas en efectivo de la sesión']
  IngresosDigitales decimal(18,2) [not null, default: 0,
    note: 'Suma de ventas por QR/transferencia']
  EgresosGastos decimal(18,2) [not null, default: 0,
    note: 'Suma de GastoOperativo.Monto de la sesión']
  EgresosPagoProveedor decimal(18,2) [not null, default: 0,
    note: 'Suma de PagoCompra de la sesión. Resta del efectivo esperado']
  MontoEsperadoEfectivo decimal(18,2) [null,
    note: 'Calculado al cierre: MontoInicial + IngresosEfectivo - EgresosGastos - EgresosPagoProveedor']
  MontoRealEntregado decimal(18,2) [null,
    note: 'Lo que el vendedor entregó físicamente']
  Diferencia decimal(18,2) [null,
    note: 'MontoRealEntregado - MontoEsperadoEfectivo']
  Estado int [not null, default: 1,
    note: '1:Abierta, 2:Cerrada, 3:Ajustada']
  CreatedAt datetime2 [not null, default: `GETDATE()`]
  UpdatedAt datetime2 [null]
}

Table Venta {
  Id int [pk, increment]
  UsuarioId int [not null, note: 'ID del JWT, sin FK']
  AlmacenId int [not null, note: 'ID del JWT/DB_Auth, sin FK']
  SesionCajaId int [not null]
  FechaVenta datetime2 [not null, default: `GETDATE()`]
  TotalBruto decimal(18,2) [not null]
  DescuentoTotal decimal(18,2) [not null, default: 0]
  TotalNeto decimal(18,2) [not null,
    note: 'TotalBruto - DescuentoTotal. Campo calculado, no editable']
  MetodoPago int [not null,
    note: '1:Efectivo, 2:TransferenciaQR, 3:Mixto']
  MotivoDescuento nvarchar(255) [null,
    note: 'Obligatorio si DescuentoTotal > 0']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
}

Table VentaDetalle {
  Id int [pk, increment]
  VentaId int [not null]
  VarianteId int [not null]
  LoteId int [not null,
    note: 'Lote específico del que salió esta unidad (FIFO)']
  CantidadUnidades int [not null]
  PrecioUnitarioCobrado decimal(18,2) [not null,
    note: 'Precio vigente al momento de la venta. Histórico inmutable']
  CostoUnitarioLote decimal(18,4) [not null,
    note: 'Costo del lote al momento de la venta. Para calcular margen']
  Subtotal decimal(18,2) [not null,
    note: 'CantidadUnidades * PrecioUnitarioCobrado. Campo calculado']
}

Table CategoriaGasto {
  Id int [pk, increment]
  Nombre nvarchar(100) [not null, unique,
    note: 'Luz, Agua, Alquiler, Sueldo, Combustible, Repuesto, Otro']
  IsActive bit [not null, default: true]
  CreatedAt datetime2 [not null, default: `GETDATE()`]
}

Table GastoOperativo {
  Id int [pk, increment]
  SesionCajaId int [not null]
  CategoriaGastoId int [not null]
  Monto decimal(18,2) [not null]
  Descripcion nvarchar(500) [null]
  EsPagoProveedor bit [not null, default: false,
    note: 'true = este gasto es un pago a proveedor que sale de la caja']
  PagoCompraId int [null,
    note: 'FK a PagoCompra si EsPagoProveedor = true']
  FechaGasto datetime2 [not null, default: `GETDATE()`]
  RegistradoByUsuarioId int [not null, note: 'ID del JWT, sin FK']
}

Ref: Venta.SesionCajaId > SesionCaja.Id
Ref: VentaDetalle.VentaId > Venta.Id
Ref: VentaDetalle.VarianteId > ProductoVariante.Id
Ref: VentaDetalle.LoteId > InventarioLote.Id
Ref: GastoOperativo.SesionCajaId > SesionCaja.Id
Ref: GastoOperativo.CategoriaGastoId > CategoriaGasto.Id
Ref: GastoOperativo.PagoCompraId > PagoCompra.Id
```

---

## Estado de Implementación

| Módulo | Tabla | Estado Backend |
|--------|-------|----------------|
| **Catálogo** | | |
| | Categoria | ✅ |
| | Fabricante | ✅ |
| | DescripcionSabor | ✅ |
| | DescripcionTamanio | ✅ |
| | TipoPresentacion | ✅ |
| | Producto | ✅ |
| | ProductoVariante | ✅ |
| | ProductoPresentacion | ✅ |
| | HistorialPrecio | ⏳ |
| **Proveedores** | | |
| | Proveedor | ✅ |
| **Inventario** | | |
| | InventarioLote | ⏳ |
| | MovimientoInventario | ⏳ |
| | Traslado | ⏳ |
| **Compras** | | |
| | Compra | ⏳ |
| | CompraDetalle | ⏳ |
| | PagoCompra | ⏳ |
| **Ventas y Caja** | | |
| | SesionCaja | ⏳ |
| | Venta | ⏳ |
| | VentaDetalle | ⏳ |
| | CategoriaGasto | ⏳ |
| | GastoOperativo | ⏳ |

---

## Notas Técnicas

### Auditoría de Fechas
Todos los campos `CreatedAt` usan `DateTime.UtcNow` en el backend. El frontend debe convertir a hora local (America/La_Paz para Bolivia).

### Campos sin FK
Los campos `UsuarioId`, `AlmacenId`, `CreatedByUsuarioId` son `int` simples que vienen del JWT token. NO tienen FK hacia DB_Auth.

### FIFO (First In, First Out)
El inventario usa método FIFO: los lotes más antiguos se venden primero. Cada `VentaDetalle` registra el `LoteId` específico del que salió la unidad.

### Enums del Sistema

| Enum | Valores | Uso |
|------|---------|-----|
| `EstadoPago` | 1:Pagado, 2:Credito, 3:ParcialmentePagado | Compra.EstadoPago |
| `TipoMovimiento` | 1:EntradaCompra, 2:SalidaVenta, 3:Traslado, 4:AjustePositivo, 5:AjusteNegativo | MovimientoInventario.TipoMovimiento |
| `MetodoPago` | 1:Efectivo, 2:TransferenciaQR, 3:Mixto | Venta.MetodoPago |
| `EstadoSesionCaja` | 1:Abierta, 2:Cerrada, 3:Ajustada | SesionCaja.Estado |
