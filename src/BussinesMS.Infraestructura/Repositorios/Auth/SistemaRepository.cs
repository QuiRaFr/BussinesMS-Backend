using BussinesMS.Aplicacion.Interfaces.Auth;
using BussinesMS.Aplicacion.Seguridad;
using BussinesMS.Dominio.Entidades.Auth;
using BussinesMS.Infraestructura.Persistencia;
using BussinesMS.Infraestructura.Repositorios.Compartido;
using SistemaEntity = BussinesMS.Dominio.Entidades.Auth.Sistema;

namespace BussinesMS.Infraestructura.Repositorios.Auth;

public class SistemaRepository : RepositorioBase<SistemaEntity>, ISistemaRepository
{
    public SistemaRepository(AuthDbContext contexto, ICurrentUserService currentUser) : base(contexto, currentUser)
    {
    }
}