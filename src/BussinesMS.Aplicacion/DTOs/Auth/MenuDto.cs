namespace BussinesMS.Aplicacion.DTOs.Auth;

public class MenuDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    public bool IsGroup { get; set; }
    public int? ParentId { get; set; }
    public int? SistemaId { get; set; }
}

public class CrearMenuDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public int? Orden { get; set; }
    public bool IsGroup { get; set; }
    public int? ParentId { get; set; }
    public int SistemaId { get; set; }
}