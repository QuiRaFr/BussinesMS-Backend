namespace BussinesMS.Aplicacion.DTOs.Auth;

public class SistemaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearSistemaDto
{
    public string Nombre { get; set; } = string.Empty;
}