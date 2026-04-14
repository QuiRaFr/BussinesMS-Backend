namespace BussinesMS.Aplicacion.DTOs.Auth;

public class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? MenuIds { get; set; }
}

public class CrearRolDto
{
    public string Nombre { get; set; } = string.Empty;
}