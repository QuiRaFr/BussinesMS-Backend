using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.EntityFrameworkCore;

namespace BussinesMS.Infraestructura.Persistencia;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Almacen> Almacenes => Set<Almacen>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<UsuarioMenu> UsuarioMenus => Set<UsuarioMenu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sistema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MenuIds).HasColumnType("nvarchar(max)");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Almacen>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(200);
            entity.Property(e => e.Icono).HasMaxLength(100);

            entity.HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Sistema)
                .WithMany()
                .HasForeignKey(m => m.SistemaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UsuarioMenu>(entity =>
        {
            entity.HasKey(e => new { e.UsuarioId, e.MenuId });

            entity.HasOne(um => um.Usuario)
                .WithMany(u => u.UsuarioMenus)
                .HasForeignKey(um => um.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(um => um.Menu)
                .WithMany()
                .HasForeignKey(um => um.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.PermisosEspeciales)
                .HasColumnType("nvarchar(max)");
        });
    }
}

public class SistemaDbContext : DbContext
{
    public SistemaDbContext(DbContextOptions<SistemaDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Fabricante> Fabricantes => Set<Fabricante>();
    public DbSet<DescripcionSabor> DescripcionSabores => Set<DescripcionSabor>();
    public DbSet<DescripcionTamanio> DescripcionTamanios => Set<DescripcionTamanio>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductoVariante> ProductoVariantes => Set<ProductoVariante>();
    public DbSet<TipoPresentacion> TiposPresentacion => Set<TipoPresentacion>();
    public DbSet<ProductoPresentacion> ProductoPresentaciones => Set<ProductoPresentacion>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> CompraDetalles => Set<CompraDetalle>();
    public DbSet<PagoCompra> PagosCompra => Set<PagoCompra>();
    public DbSet<InventarioLote> InventarioLotes => Set<InventarioLote>();
    public DbSet<InventarioLoteAlmacen> InventarioLoteAlmacenes => Set<InventarioLoteAlmacen>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<Traslado> Traslados => Set<Traslado>();
    public DbSet<TrasladoDetalle> TrasladosDetalles => Set<TrasladoDetalle>();
    public DbSet<DevolucionCliente> DevolucionesClientes => Set<DevolucionCliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Fabricante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoInterno).HasMaxLength(20);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.CodigoInterno).IsUnique();
            entity.HasIndex(e => e.Nombre);

            entity.HasOne(p => p.Categoria)
                .WithMany()
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Fabricante)
                .WithMany()
                .HasForeignKey(p => p.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductoVariante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreProducto).HasMaxLength(200);
            entity.Property(e => e.DescripcionProducto).HasMaxLength(500);
            entity.Property(e => e.CodigoBarras).HasMaxLength(50);
            entity.Property(e => e.SaborDescripcion).HasMaxLength(200);
            entity.Property(e => e.PesoTamanio).HasMaxLength(100);
            entity.Property(e => e.PrecioVentaUnitario).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.PrecioVentaMayoreo).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.PrecioCompra).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.CodigoAlmacen).HasMaxLength(50);

            entity.HasIndex(e => e.CodigoBarras).IsUnique().HasFilter("[CodigoBarras] IS NOT NULL");
            entity.HasIndex(e => new { e.ProductoId, e.SaborId, e.TamanioId })
                  .IsUnique()
                  .HasDatabaseName("UQ_Variante_Combinacion");

            entity.HasOne(pv => pv.Producto)
                .WithMany()
                .HasForeignKey(pv => pv.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pv => pv.Sabor)
                .WithMany()
                .HasForeignKey(pv => pv.SaborId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pv => pv.Tamanio)
                .WithMany()
                .HasForeignKey(pv => pv.TamanioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoPresentacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasIndex(e => e.Orden).IsUnique();
        });

        modelBuilder.Entity<ProductoPresentacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombrePersonalizado).HasMaxLength(50);
            entity.Property(e => e.CodigoBarras).HasMaxLength(50);
            entity.HasIndex(e => e.CodigoBarras)
                  .IsUnique()
                  .HasFilter("[CodigoBarras] IS NOT NULL AND [IsActive] = 1");

            entity.HasIndex(e => new { e.VarianteId, e.TipoPresentacionId })
                  .IsUnique()
                  .HasFilter("[IsActive] = 1")
                  .HasDatabaseName("UQ_Presentacion_Variante");

            entity.HasIndex(e => e.VarianteId)
                  .IsUnique()
                  .HasFilter("[EsDefaultReporte] = 1 AND [IsActive] = 1")
                  .HasDatabaseName("UQ_Presentacion_DefaultReporte");

            entity.HasOne(pp => pp.Variante)
                .WithMany(pv => pv.Presentaciones)
                .HasForeignKey(pp => pp.VarianteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pp => pp.TipoPresentacion)
                .WithMany(tp => tp.Presentaciones)
                .HasForeignKey(pp => pp.TipoPresentacionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pp => pp.PresentacionPadre)
                .WithMany(pp => pp.PresentacionesHijas)
                .HasForeignKey(pp => pp.PresentacionPadreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Nit).HasMaxLength(20);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalCompra).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Observacion).HasMaxLength(500);

            entity.HasOne(c => c.Proveedor)
                .WithMany()
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CompraDetalle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CostoUnitario).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.Subtotal).IsRequired().HasColumnType("decimal(18,2)");

            entity.HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Variante)
                .WithMany()
                .HasForeignKey(d => d.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PagoCompra>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Observacion).HasMaxLength(255);
            entity.Property(e => e.PagadoPorUsuarioId).IsRequired();

            entity.HasOne(p => p.Compra)
                .WithMany(c => c.Pagos)
                .HasForeignKey(p => p.CompraId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =============================================
        // INVENTARIO — InventarioLote (IDENTIDAD)
        // =============================================
        modelBuilder.Entity<InventarioLote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CostoCompraUnitario).IsRequired().HasColumnType("decimal(18,4)");
            entity.Property(e => e.CantidadTotal).IsRequired();
            entity.Property(e => e.FechaVencimiento).HasColumnType("date");

            entity.HasOne(l => l.Variante)
                .WithMany()
                .HasForeignKey(l => l.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.CompraDetalle)
                .WithMany()
                .HasForeignKey(l => l.CompraDetalleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // =============================================
        // INVENTARIO — InventarioLoteAlmacen (STOCK/UBICACIÓN)
        // =============================================
        modelBuilder.Entity<InventarioLoteAlmacen>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.LoteId, e.AlmacenId })
                  .IsUnique()
                  .HasDatabaseName("UQ_LoteAlmacen");

            entity.HasIndex(e => new { e.AlmacenId, e.LoteId })
                  .HasDatabaseName("IX_LoteAlmacen_FEFO");

            entity.HasIndex(e => new { e.VarianteId, e.AlmacenId })
                  .HasDatabaseName("IX_LoteAlmacen_VarianteAlmacen");

            entity.HasOne(la => la.Lote)
                .WithMany()
                .HasForeignKey(la => la.LoteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =============================================
        // MOVIMIENTOS — MovimientoInventario
        // =============================================
        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Observacion).HasMaxLength(255);
            entity.Property(e => e.FechaMovimiento).HasColumnType("datetime2");

            entity.HasIndex(e => e.LoteAlmacenId);
            entity.HasIndex(e => e.VarianteId);
            entity.HasIndex(e => new { e.AlmacenOrigenId, e.AlmacenDestinoId });

            entity.HasOne(m => m.LoteAlmacen)
                .WithMany()
                .HasForeignKey(m => m.LoteAlmacenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Variante)
                .WithMany()
                .HasForeignKey(m => m.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =============================================
        // TRASLADOS — Cabecera + Detalle
        // =============================================
        modelBuilder.Entity<Traslado>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Observacion).HasMaxLength(255);
            entity.Property(e => e.FechaTraslado).HasColumnType("datetime2");

            entity.HasOne(t => t.Variante)
                .WithMany()
                .HasForeignKey(t => t.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TrasladoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CostoUnitarioCapturado).HasColumnType("decimal(18,4)");

            entity.HasOne(td => td.Traslado)
                .WithMany(t => t.Detalles)
                .HasForeignKey(td => td.TrasladoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(td => td.LoteAlmacenOrigen)
                .WithMany()
                .HasForeignKey(td => td.LoteAlmacenOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(td => td.LoteAlmacenDestino)
                .WithMany()
                .HasForeignKey(td => td.LoteAlmacenDestinoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // =============================================
        // DEVOLUCIONES — DevolucionCliente
        // =============================================
        modelBuilder.Entity<DevolucionCliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Motivo).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Observacion).HasMaxLength(255);
            entity.Property(e => e.FechaDevolucion).HasColumnType("datetime2");

            entity.HasOne(d => d.LoteAlmacenOrigen)
                .WithMany()
                .HasForeignKey(d => d.LoteAlmacenOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LoteAlmacenDevuelto)
                .WithMany()
                .HasForeignKey(d => d.LoteAlmacenDevueltoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Variante)
                .WithMany()
                .HasForeignKey(d => d.VarianteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

public class NavidadDbContext : DbContext
{
    public NavidadDbContext(DbContextOptions<NavidadDbContext> opciones) : base(opciones)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
