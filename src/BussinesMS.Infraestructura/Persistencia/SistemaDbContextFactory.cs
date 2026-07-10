using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BussinesMS.Infraestructura.Persistencia;

public class SistemaDbContextFactory : IDesignTimeDbContextFactory<SistemaDbContext>
{
    public SistemaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SistemaDbContext>();
        optionsBuilder.UseSqlServer("Server=LAPTOP-NNA8N5KQ\\SQLEXPRESS;Database=BussinesMS_Sistema;User Id=sa;Password=12345678;TrustServerCertificate=True;");
        return new SistemaDbContext(optionsBuilder.Options);
    }
}
