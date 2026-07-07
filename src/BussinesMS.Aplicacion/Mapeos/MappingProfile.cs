using AutoMapper;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Sistema;
using BussinesMS.Aplicacion.Common;

namespace BussinesMS.Aplicacion.Mapeos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Sistemas
        CreateMap<Sistema, SistemaDto>();
        CreateMap<SistemaDto, Sistema>();
        CreateMap<CrearSistemaDto, Sistema>();

        // Roles
        CreateMap<Rol, RolDto>();
        CreateMap<RolDto, Rol>();
        CreateMap<CrearRolDto, Rol>();

        // Almacenes
        CreateMap<Almacen, AlmacenDto>();
        CreateMap<AlmacenDto, Almacen>();
        CreateMap<CrearAlmacenDto, Almacen>();

        // Usuarios
        CreateMap<Usuario, UsuarioDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<UsuarioDto, Usuario>();
        CreateMap<CrearUsuarioDto, Usuario>();

        // Menus
        CreateMap<Menu, MenuDto>();
        CreateMap<MenuDto, Menu>();
        CreateMap<CrearMenuDto, Menu>();

        // Categorías
        CreateMap<Categoria, CategoriaDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<CategoriaDto, Categoria>();
        CreateMap<CrearCategoriaDto, Categoria>();
        CreateMap<ActualizarCategoriaDto, Categoria>();

        // Fabricantes
        CreateMap<Fabricante, FabricanteDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<FabricanteDto, Fabricante>();
        CreateMap<CrearFabricanteDto, Fabricante>();
        CreateMap<ActualizarFabricanteDto, Fabricante>();

        // Sabores
        CreateMap<DescripcionSabor, DescripcionSaborDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<DescripcionSaborDto, DescripcionSabor>();
        CreateMap<CrearDescripcionSaborDto, DescripcionSabor>();
        CreateMap<ActualizarDescripcionSaborDto, DescripcionSabor>();

        // Tamaños
        CreateMap<DescripcionTamanio, DescripcionTamanioDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<DescripcionTamanioDto, DescripcionTamanio>();
        CreateMap<CrearDescripcionTamanioDto, DescripcionTamanio>();
        CreateMap<ActualizarDescripcionTamanioDto, DescripcionTamanio>();

        // Productos
        CreateMap<Producto, ProductoDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<ProductoDto, Producto>();
        CreateMap<CrearProductoDto, Producto>();
        CreateMap<ActualizarProductoDto, Producto>();

        // Proveedores
        CreateMap<Proveedor, ProveedorDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<ProveedorDto, Proveedor>();
        CreateMap<CrearProveedorDto, Proveedor>();
        CreateMap<ActualizarProveedorDto, Proveedor>();

        // ProductoVariantes
        CreateMap<ProductoVariante, ProductoVarianteDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<ProductoVarianteDto, ProductoVariante>();
        CreateMap<CrearProductoVarianteDto, ProductoVariante>();
        CreateMap<ActualizarProductoVarianteDto, ProductoVariante>();

        // Compras
        CreateMap<Compra, CompraDto>()
            .ForMember(dest => dest.ProveedorNombre, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null))
            .ForMember(dest => dest.ProveedorNit, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nit : null))
            .ForMember(dest => dest.ProveedorTelefono, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Telefono : null))
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId))
            .ForMember(dest => dest.FechaCompra, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.FechaCompra)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<CompraDto, Compra>();
        CreateMap<CrearCompraDto, Compra>()
            .ForMember(dest => dest.Detalles, opt => opt.Ignore());
        CreateMap<ActualizarCompraDto, Compra>();

        // CompraDetalles
        CreateMap<CompraDetalle, CompraDetalleDto>()
            .ForMember(dest => dest.VarianteNombre, opt => opt.MapFrom(src => src.Variante != null ? src.Variante.DescripcionProducto : null))
            .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.FechaVencimiento.HasValue ? BoliviaTimeZone.ToLocal(src.FechaVencimiento.Value) : (DateTime?)null));
        // PagosCompra
        CreateMap<PagoCompra, PagoCompraDto>()
            .ForMember(dest => dest.FechaPago, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.FechaPago)));
        CreateMap<CrearPagoCompraDto, PagoCompra>();

        // TipoPresentacion
        CreateMap<TipoPresentacion, TipoPresentacionDto>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
        CreateMap<CrearTipoPresentacionDto, TipoPresentacion>();
        CreateMap<ActualizarTipoPresentacionDto, TipoPresentacion>();

        // ProductoPresentacion — el DTO se construye manualmente en el service
        // por eso solo necesitamos el mapeo inverso
        CreateMap<CrearProductoPresentacionDto, ProductoPresentacion>();
        CreateMap<ActualizarProductoPresentacionDto, ProductoPresentacion>();

        // InventarioLoteAlmacen
        CreateMap<InventarioLoteAlmacen, InventarioLoteAlmacenDto>();

        // MovimientoInventario
        CreateMap<MovimientoInventario, MovimientoInventarioDto>()
            .ForMember(dest => dest.FechaMovimiento, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.FechaMovimiento)));

        // Traslado
        CreateMap<Traslado, TrasladoDto>()
            .ForMember(dest => dest.FechaTraslado, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.FechaTraslado)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));

        // DevolucionCliente
        CreateMap<DevolucionCliente, DevolucionClienteDto>()
            .ForMember(dest => dest.FechaDevolucion, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.FechaDevolucion)))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => BoliviaTimeZone.ToLocal(src.CreatedAt)));
    }
}
