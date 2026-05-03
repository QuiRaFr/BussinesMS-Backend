using BussinesMS.Aplicacion.Interfaces.Sistema;
using BussinesMS.Infraestructura.Persistencia;

namespace BussinesMS.Infraestructura.Persistencia;

public class SistemaUnitOfWork : ISistemaUnitOfWork
{
    private readonly SistemaDbContext _context;

    public SistemaUnitOfWork(SistemaDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}