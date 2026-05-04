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

        // ProductoVariantes
        CreateMap<ProductoVariante, ProductoVarianteDto>()
            .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null))
            .ForMember(dest => dest.SaborNombre, opt => opt.MapFrom(src => src.Sabor != null ? src.Sabor.Nombre : null))
            .ForMember(dest => dest.TamanioNombre, opt => opt.MapFrom(src => src.Tamanio != null ? src.Tamanio.Nombre : null));
        CreateMap<ProductoVarianteDto, ProductoVariante>();
        CreateMap<CrearProductoVarianteDto, ProductoVariante>();
        CreateMap<ActualizarProductoVarianteDto, ProductoVariante>();
    }
}