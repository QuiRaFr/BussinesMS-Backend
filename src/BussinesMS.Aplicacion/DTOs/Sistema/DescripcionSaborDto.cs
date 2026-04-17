namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class DescripcionSaborDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearDescripcionSaborDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class ActualizarDescripcionSaborDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}