namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class ProveedorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Telefono { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearProveedorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Telefono { get; set; }
}

public class ActualizarProveedorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Nit { get; set; }
    public string? Telefono { get; set; }
    public bool IsActive { get; set; }
}
