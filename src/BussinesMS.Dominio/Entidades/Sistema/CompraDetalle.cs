namespace BussinesMS.Dominio.Entidades.Sistema;

public class CompraDetalle
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public int VarianteId { get; set; }
    public int CantidadUnidades { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime? FechaVencimiento { get; set; }

    public Compra? Compra { get; set; }
    public ProductoVariante? Variante { get; set; }
}
