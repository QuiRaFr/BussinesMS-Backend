namespace BussinesMS.Aplicacion.Interfaces.Sistema;

public interface ISistemaUnitOfWork
{
    Task BeginTransactionAsync();
    Task<int> SaveChangesAsync();
    Task CommitAsync();
    Task RollbackAsync();
}