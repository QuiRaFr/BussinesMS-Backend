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
    }
}