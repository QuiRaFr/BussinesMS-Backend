namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class TipoPresentacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Factor { get; set; }
    public bool Activo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearTipoPresentacionDto
{
    public string Nombre { get; set; } = string.Empty;
    public int Factor { get; set; }
}

public class ActualizarTipoPresentacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Factor { get; set; }
    public bool Activo { get; set; }
}