namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class DescripcionTamanioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearDescripcionTamanioDto
{
    public string Nombre { get; set; } = string.Empty;
}

public class ActualizarDescripcionTamanioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}