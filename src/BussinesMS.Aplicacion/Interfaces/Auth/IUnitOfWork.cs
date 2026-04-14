namespace BussinesMS.Aplicacion.Interfaces.Auth;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}