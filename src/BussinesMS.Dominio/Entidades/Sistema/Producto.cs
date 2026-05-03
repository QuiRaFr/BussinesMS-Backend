using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Sistema;

public class Producto : EntidadBase
{
    public string? CodigoInterno { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public int FabricanteId { get; set; }
    public Fabricante? Fabricante { get; set; }
}