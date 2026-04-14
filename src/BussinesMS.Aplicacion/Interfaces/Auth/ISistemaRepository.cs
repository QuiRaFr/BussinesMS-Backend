using BussinesMS.Aplicacion.Interfaces.Compartido;
using BussinesMS.Dominio.Entidades.Auth;
using SistemaEntity = BussinesMS.Dominio.Entidades.Auth.Sistema;

namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface ISistemaRepository : IRepositorio<SistemaEntity>
{
}