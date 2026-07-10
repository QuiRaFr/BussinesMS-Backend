using BussinesMS.Dominio.Excepciones;
using BussinesMS.Dominio.Interfaces;

namespace BussinesMS.Aplicacion.Helpers;

public static class ValidacionEntidad
{
    public static void VerificarExiste<T>(T? entidad, string nombre) where T : class
    {
        if (entidad == null)
            throw new EntidadNoEncontradaException(nombre, 0);
    }

    public static void VerificarActivo<T>(T? entidad, string nombre)
        where T : class, IEntidadActivable
    {
        if (entidad == null)
            throw new EntidadNoEncontradaException(nombre, 0);

        if (!entidad.IsActive)
            throw new ValidacionException($"{nombre} no está activo");
    }

    public static void VerificarNoDuplicado(bool existe, string nombreEntidad, string nombre)
    {
        if (existe)
            throw new EntidadDuplicadaException(nombreEntidad, nombre);
    }
}
