using System.ComponentModel.DataAnnotations;

namespace BussinesMS.Aplicacion.DTOs.Plantillas;

public enum TipoCategoriaFiltro
{
    Todos = 0,
    Categoria = 1,
    Subcategoria = 2
}

public class GenericPaginationQueryDto : IValidatableObject
{
    public bool? IsPaged { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public string? Filter { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public string? FieldValue { get; set; }
    public string? FieldName { get; set; }
    public TipoCategoriaFiltro? FiltroTipo { get; set; }

    public bool GetIsPagedValue() => IsPaged ?? true;
    public int GetPageValue() => Page ?? 1;
    public int GetPageSizeValue() => PageSize ?? 10;
    public string GetSortDirectionValue() => SortDirection ?? "desc";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (GetIsPagedValue())
        {
            if (Page.HasValue && Page.Value <= 0)
                yield return new ValidationResult("Page debe ser mayor que 0.", new[] { nameof(Page) });

            if (PageSize.HasValue && PageSize.Value <= 0)
                yield return new ValidationResult("PageSize debe ser mayor que 0.", new[] { nameof(PageSize) });
            
            if (PageSize.HasValue && PageSize.Value > 100)
                yield return new ValidationResult("PageSize no puede ser mayor que 100.", new[] { nameof(PageSize) });
        }
    }
}