namespace BussinesMS.Aplicacion.DTOs.Sistema.Migracion;

public class FilaCsv
{
    public string? CodigoBarras { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Subcategoria { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public string? SaborDescripcion { get; set; }
    public string? PesoTamanio { get; set; }
    public string? Fabrica { get; set; }
    public string? Unidad { get; set; }
    public string? Display { get; set; }
    public string? Caja { get; set; }
}

public class ResultadoMigracionDto
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int FilasProcesadas { get; set; }
    public int Creadas { get; set; }
    public int Omitidas { get; set; }
    public List<string> CategoriasCreadas { get; set; } = new();
    public List<string> SubcategoriasCreadas { get; set; } = new();
    public List<string> FabricantesCreados { get; set; } = new();
    public List<string> SaboresCreados { get; set; } = new();
    public List<string> TamaniosCreados { get; set; } = new();
    public List<string> PresentacionesCreadas { get; set; } = new();
}