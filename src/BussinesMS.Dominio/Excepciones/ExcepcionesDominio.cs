namespace BussinesMS.Dominio.Excepciones;

public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje) { }
}

public class EntidadNoEncontradaException : ExcepcionDominio
{
    public EntidadNoEncontradaException(string nombreEntidad, int id) 
        : base($"{nombreEntidad} con ID {id} no encontrada") { }
}

public class ValidacionException : ExcepcionDominio
{
    public ValidacionException(string mensaje) : base(mensaje) { }
}