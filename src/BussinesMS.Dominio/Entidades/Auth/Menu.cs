using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Auth;

public class Menu : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; } = 0;
    public bool IsGroup { get; set; } = false;

    // Relación recursiva
    public int? ParentId { get; set; }
    public Menu? Parent { get; set; }
    public ICollection<Menu> Children { get; set; } = new List<Menu>();

    public int? SistemaId { get; set; }
    public Sistema? Sistema { get; set; }
}