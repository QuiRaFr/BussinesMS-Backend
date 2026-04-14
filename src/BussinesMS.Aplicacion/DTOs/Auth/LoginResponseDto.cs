using BussinesMS.Aplicacion.DTOs.Auth;

namespace BussinesMS.Aplicacion.DTOs.Auth;

public class LoginResponseDto
{
    public UsuarioDto Usuario { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public string? RolNombre { get; set; }
    public List<MenuPermisoDto> Menus { get; set; } = new();
}