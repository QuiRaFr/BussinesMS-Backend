using BussinesMS.Aplicacion.DTOs.Plantillas;

namespace BussinesMS.Aplicacion.DTOs.Sistema;

public class ProductoPaginationQueryDto : GenericPaginationQueryDto
{
    public int? CategoriaId { get; set; }
    public int? SubcategoriaId { get; set; }
}