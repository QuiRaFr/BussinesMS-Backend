namespace BussinesMS.Aplicacion.Helpers;

public static class ValidacionEntidad
{
    public static void VerificarExiste<T>(T? entidad, string nombre) where T : class
    {
        if (entidad == null)
            throw new Exception($"{nombre} no existe");
    }

    public static void VerificarActivo<T>(T? entidad, string nombre) where T : class
    {
        if (entidad == null)
            throw new Exception($"{nombre} no existe");
        
        var propiedad = entidad.GetType().GetProperty("IsActive");
        if (propiedad?.GetValue(entidad) is false)
            throw new Exception($"{nombre} no está activo");
    }
}