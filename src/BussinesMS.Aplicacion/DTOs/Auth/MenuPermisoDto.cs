namespace BussinesMS.Aplicacion.DTOs.Auth;

public class MenuPermisoDto
{
    public int MenuId { get; set; }
    public string? Nombre { get; set; }
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public string? JerarquiaName { get; set; }
    public int SistemaId { get; set; }
    public bool Leer { get; set; }
    public bool Crear { get; set; }
    public bool Editar { get; set; }
    public bool Eliminar { get; set; }
}

public class MenuPermisoSimpleDto
{
    public int MenuId { get; set; }
    public bool Leer { get; set; }
    public bool Crear { get; set; }
    public bool Editar { get; set; }
    public bool Eliminar { get; set; }
}

public class ActualizarMenusDto
{
    public List<MenuPermisoSimpleDto> Menus { get; set; } = new();
}

public class RolMenuIdsDto
{
    public List<int> MenuIds { get; set; } = new();
}