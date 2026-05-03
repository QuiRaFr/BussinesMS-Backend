namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ISistemaUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}