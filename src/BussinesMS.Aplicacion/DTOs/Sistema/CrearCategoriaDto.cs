namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class CrearCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

public class ActualizarCategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}
