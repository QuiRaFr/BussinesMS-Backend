using AutoMapper;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Dominio.Entidades.Sistema;
using BussinesMS.Aplicacion.DTOs.Auth;
using BussinesMS.Aplicacion.DTOs.Sistema;

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
        CreateMap<Usuario, UsuarioDto>();
        CreateMap<UsuarioDto, Usuario>();
        CreateMap<CrearUsuarioDto, Usuario>();

        // Menus
        CreateMap<Menu, MenuDto>();
        CreateMap<MenuDto, Menu>();
        CreateMap<CrearMenuDto, Menu>();

        // Categorías
        CreateMap<Categoria, CategoriaDto>();
        CreateMap<CategoriaDto, Categoria>();
        CreateMap<CrearCategoriaDto, Categoria>();
        CreateMap<ActualizarCategoriaDto, Categoria>();

        // Fabricantes
        CreateMap<Fabricante, FabricanteDto>();
        CreateMap<FabricanteDto, Fabricante>();
        CreateMap<CrearFabricanteDto, Fabricante>();
        CreateMap<ActualizarFabricanteDto, Fabricante>();

        // Sabores
        CreateMap<DescripcionSabor, DescripcionSaborDto>();
        CreateMap<DescripcionSaborDto, DescripcionSabor>();
        CreateMap<CrearDescripcionSaborDto, DescripcionSabor>();
        CreateMap<ActualizarDescripcionSaborDto, DescripcionSabor>();

        // Tamaños
        CreateMap<DescripcionTamanio, DescripcionTamanioDto>();
        CreateMap<DescripcionTamanioDto, DescripcionTamanio>();
        CreateMap<CrearDescripcionTamanioDto, DescripcionTamanio>();
        CreateMap<ActualizarDescripcionTamanioDto, DescripcionTamanio>();

        // Productos
        CreateMap<Producto, ProductoDto>();
        CreateMap<ProductoDto, Producto>();
        CreateMap<CrearProductoDto, Producto>();
        CreateMap<ActualizarProductoDto, Producto>();

        // Proveedores
        CreateMap<Proveedor, ProveedorDto>();
        CreateMap<ProveedorDto, Proveedor>();
        CreateMap<CrearProveedorDto, Proveedor>();
        CreateMap<ActualizarProveedorDto, Proveedor>();

        // ProductoVariantes
        CreateMap<ProductoVariante, ProductoVarianteDto>();
        CreateMap<ProductoVarianteDto, ProductoVariante>();
        CreateMap<CrearProductoVarianteDto, ProductoVariante>();
        CreateMap<ActualizarProductoVarianteDto, ProductoVariante>();

        // Compras
        CreateMap<Compra, CompraDto>()
            .ForMember(dest => dest.ProveedorNombre, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nombre : null))
            .ForMember(dest => dest.ProveedorNit, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Nit : null))
            .ForMember(dest => dest.ProveedorTelefono, opt => opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Telefono : null))
            .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId));
        CreateMap<CompraDto, Compra>();
        CreateMap<CrearCompraDto, Compra>()
            .ForMember(dest => dest.Detalles, opt => opt.Ignore());
        CreateMap<ActualizarCompraDto, Compra>();

        // CompraDetalles
        CreateMap<CompraDetalle, CompraDetalleDto>()
            .ForMember(dest => dest.VarianteNombre, opt => opt.MapFrom(src => src.Variante != null ? src.Variante.DescripcionProducto : null));
        CreateMap<CrearCompraDetalleDto, CompraDetalle>();

        // PagosCompra
        CreateMap<PagoCompra, PagoCompraDto>();
        CreateMap<CrearPagoCompraDto, PagoCompra>();

        // TipoPresentacion
        CreateMap<TipoPresentacion, TipoPresentacionDto>();
        CreateMap<CrearTipoPresentacionDto, TipoPresentacion>();
        CreateMap<ActualizarTipoPresentacionDto, TipoPresentacion>();

        // ProductoPresentacion — el DTO se construye manualmente en el service
        // por eso solo necesitamos el mapeo inverso
        CreateMap<CrearProductoPresentacionDto, ProductoPresentacion>();
        CreateMap<ActualizarProductoPresentacionDto, ProductoPresentacion>();

        // InventarioLote
        CreateMap<InventarioLote, InventarioLoteDto>()
            .ForMember(dest => dest.VarianteNombre,
                opt => opt.MapFrom(src => src.Variante != null ? src.Variante.DescripcionProducto : null))
            .ForMember(dest => dest.NombreProducto,
                opt => opt.MapFrom(src => src.Variante != null && src.Variante.Producto != null ? src.Variante.Producto.Nombre : null))
            .ForMember(dest => dest.CodigoBarras,
                opt => opt.MapFrom(src => src.Variante != null ? src.Variante.CodigoBarras : null))
            .ForMember(dest => dest.CategoriaId,
                opt => opt.MapFrom(src => src.Variante != null && src.Variante.Producto != null ? (int?)src.Variante.Producto.CategoriaId : (int?)null))
            .ForMember(dest => dest.CategoriaNombre,
                opt => opt.MapFrom(src => src.Variante != null && src.Variante.Producto != null && src.Variante.Producto.Categoria != null ? src.Variante.Producto.Categoria.Nombre : null))
            .ForMember(dest => dest.DiasParaVencer,
                opt => opt.MapFrom(src => src.FechaVencimiento != null
                    ? (int?)Math.Max(0, (src.FechaVencimiento.Value - DateTime.UtcNow).Days)
                    : null));
        CreateMap<CrearInventarioLoteDto, InventarioLote>();
        CreateMap<ActualizarInventarioLoteDto, InventarioLote>();

        // MovimientoInventario
        CreateMap<MovimientoInventario, MovimientoInventarioDto>();

        // Traslado
        CreateMap<Traslado, TrasladoDto>();

        // DevolucionCliente
        CreateMap<DevolucionCliente, DevolucionClienteDto>();
    }
}