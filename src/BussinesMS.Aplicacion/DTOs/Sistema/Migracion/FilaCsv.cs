namespace BussinesMS.Aplicacion.DTOs.Sistema.Migracion;

public class FilaCsv
{
    public string? CodigoBarras { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string NombreProducto { get; set; } = string.Empty;
    public string? Sabor { get; set; }
    public string? Tamanio { get; set; }
    public string? Fabrica { get; set; }
    public int Unidad { get; set; } = 1;
    public int? Caja { get; set; }
    public int? Caja2 { get; set; }
    public string NombreUnidad { get; set; } = "Unidad";
    public string NombreCaja { get; set; } = "Paquete";
    public string? NombreCaja2 { get; set; }
    public string DefaultReporte { get; set; } = "Unidad";
}

public class ResultadoMigracionDto
{
    public bool Success { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int FilasProcesadas { get; set; }
    public int Creadas { get; set; }
    public int Omitidas { get; set; }
    public List<string> CategoriasCreadas { get; set; } = new();
    public List<string> FabricantesCreados { get; set; } = new();
    public List<string> SaboresCreados { get; set; } = new();
    public List<string> TamaniosCreados { get; set; } = new();
    public List<string> PresentacionesCreadas { get; set; } = new();
    public List<string> ProductosCreados { get; set; } = new();
    public List<string> ProductoVariantesCreados { get; set; } = new();
    public List<string> VariantesOmitidas { get; set; } = new();
    public List<string> ProveedoresCreados { get; set; } = new();
    public List<string> TiposPresentacionCreados { get; set; } = new();
}