namespace BussinesMS.Dominio.Entidades.Compartido;

public abstract class EntidadBase
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int CreatedByUsuarioId { get; set; }
    public int? UpdatedByUsuarioId { get; set; }
    public int? DeletedByUsuarioId { get; set; }
}