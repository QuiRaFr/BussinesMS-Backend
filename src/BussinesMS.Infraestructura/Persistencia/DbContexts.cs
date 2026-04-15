using BussinesMS.Dominio.Entidades;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Dominio.Entidades.Sistema;
using Microsoft.EntityFrameworkCore;
using EntidadBase = BussinesMS.Dominio.Entidades.Compartido.EntidadBase;

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
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<PermisoRol> PermisoRoles => Set<PermisoRol>();
    public DbSet<Menu> Menus => Set<Menu>();

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
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
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

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Codigo).IsUnique();
            
            entity.HasOne(p => p.Menu)
                .WithMany()
                .HasForeignKey(p => p.MenuId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PermisoRol>(entity =>
        {
            entity.HasKey(e => new { e.PermisoId, e.RolId });
            
            entity.HasOne(pr => pr.Permiso)
                .WithMany(p => p.PermisoRoles)
                .HasForeignKey(pr => pr.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(pr => pr.Rol)
                .WithMany(r => r.PermisoRoles)
                .HasForeignKey(pr => pr.RolId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Url).HasMaxLength(200);
            entity.Property(e => e.Icono).HasMaxLength(50);
            entity.Property(e => e.JerarquiaName).HasMaxLength(100);
            
            entity.HasOne(m => m.Sistema)
                .WithMany()
                .HasForeignKey(m => m.SistemaId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(m => m.Permiso)
                .WithMany()
                .HasForeignKey(m => m.PermisoId)
                .OnDelete(DeleteBehavior.SetNull);
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.Nombre, e.ParentId }).IsUnique();

            entity.HasOne(c => c.Parent)
                .WithMany(c => c.Subcategorias)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Fabricante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.HasIndex(e => e.Nombre).IsUnique();
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