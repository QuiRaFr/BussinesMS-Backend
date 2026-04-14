namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? NombrePadre { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
