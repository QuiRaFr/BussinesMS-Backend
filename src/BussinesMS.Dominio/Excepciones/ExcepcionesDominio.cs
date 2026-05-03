namespace BussinesMS.Dominio.Excepciones;

public class ExcepcionDominio : Exception
{
    public int CodigoHttp { get; set; } = 400;
    public string CodigoError { get; set; } = "ERROR";

    public ExcepcionDominio(string mensaje, int codigoHttp = 400, string codigoError = "ERROR") 
        : base(mensaje) 
    {
        CodigoHttp = codigoHttp;
        CodigoError = codigoError;
    }
}

public class EntidadNoEncontradaException : ExcepcionDominio
{
    public EntidadNoEncontradaException(string nombreEntidad, int id) 
        : base($"{nombreEntidad} con ID {id} no encontrada", 404, "ENTIDAD_NO_ENCONTRADA") { }
}

public class ValidacionException : ExcepcionDominio
{
    public ValidacionException(string mensaje) : base(mensaje, 400, "VALIDACION_ERROR") { }
}

public class EntidadDuplicadaException : ExcepcionDominio
{
    public EntidadDuplicadaException(string nombreEntidad, string nombre) 
        : base($"Ya existe {nombreEntidad} con el nombre '{nombre}'", 409, "ENTIDAD_DUPLICADA") { }
}

public class CategoriaInvalidaException : ExcepcionDominio
{
    public CategoriaInvalidaException(string mensaje) 
        : base(mensaje, 400, "CATEGORIA_INVALIDA") { }
}