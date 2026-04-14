using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Categoria : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public Categoria? Parent { get; set; }
    public ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();
}
