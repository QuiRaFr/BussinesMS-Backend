namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class ProductoDto
{
    public int Id { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public int FabricanteId { get; set; }
    public string? FabricanteNombre { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CrearProductoDto
{
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int FabricanteId { get; set; }
}

public class ActualizarProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public int FabricanteId { get; set; }
    public bool IsActive { get; set; }
}