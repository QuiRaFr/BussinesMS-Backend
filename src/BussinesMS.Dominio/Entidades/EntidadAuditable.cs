using BussinesMS.Dominio.Entidades.Compartido;

namespace BussinesMS.Dominio.Entidades;

public abstract class EntidadAuditable : EntidadBase
{
    // Hereda: Id, IsActive, CreatedAt, UpdatedAt, DeletedAt,
    //        CreatedByUsuarioId, UpdatedByUsuarioId, DeletedByUsuarioId
}