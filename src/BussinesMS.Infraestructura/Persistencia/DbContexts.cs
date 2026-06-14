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

            // Relación recursiva padre → hijos
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
            // Clave compuesta
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

// SistemaDbContext y NavidadDbContext sin cambios
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
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<CompraDetalle> CompraDetalles => Set<CompraDetalle>();
    public DbSet<PagoCompra> PagosCompra => Set<PagoCompra>();
    public DbSet<TipoPresentacion> TiposPresentacion => Set<TipoPresentacion>();
    public DbSet<ProductoPresentacion> ProductoPresentaciones => Set<ProductoPresentacion>();

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

        // Reemplazar la configuración existente de ProductoVariante
        modelBuilder.Entity<ProductoVariante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreProducto).HasMaxLength(200);
            entity.Property(e => e.CodigoBarras).HasMaxLength(50);
            entity.Property(e => e.SaborDescripcion).HasMaxLength(200);
            entity.Property(e => e.PesoTamanio).HasMaxLength(100);
            entity.Property(e => e.PrecioVentaActual).IsRequired().HasColumnType("decimal(18,2)");
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

        // Nueva entidad TipoPresentacion
        modelBuilder.Entity<TipoPresentacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.HasIndex(e => e.Orden).IsUnique(); // cada nivel tiene orden único
        });

        // Nueva entidad ProductoPresentacion
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

            // Solo 1 EsDefaultReporte=true por VarianteId (entre las activas)
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

            // Self-referencing para la jerarquía
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

            entity.HasOne(p => p.Compra)
                .WithMany(c => c.Pagos)
                .HasForeignKey(p => p.CompraId)
                .OnDelete(DeleteBehavior.Cascade);
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