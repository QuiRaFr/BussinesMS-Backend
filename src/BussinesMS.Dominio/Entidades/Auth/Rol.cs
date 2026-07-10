using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades.Auth;

public class Rol : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    // MenuIds solo como referencia para cargar defaults en el frontend
    // Se guarda como JSON: [1, 2, 3]
    public string? MenuIds { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}