namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class FabricanteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearFabricanteDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ActualizarFabricanteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}