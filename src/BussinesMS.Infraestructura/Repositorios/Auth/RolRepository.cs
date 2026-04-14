using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Infraestructura.Persistencia;
using BussinesMS.Infraestructura.Repositorios.Compartido;

namespace BussinesMS.Infraestructura.Repositorios.Auth;

public class RolRepository : RepositorioBase<Rol>, IRolRepository
{
    public RolRepository(AuthDbContext contexto, ICurrentUserService currentUser) : base(contexto, currentUser)
    {
    }
}