namespace BussinesMS.Aplicacion.Seguridad;

public interface ICurrentUserService
{
    int? GetUsuarioId();
    string? GetUsername();
    int? GetRolId();
    void SetUser(int usuarioId, string username, int rolId);
    void Clear();
}

public class CurrentUserService : ICurrentUserService
{
    private int? _usuarioId;
    private string? _username;
    private int? _rolId;

    public int? GetUsuarioId() => _usuarioId;
    public string? GetUsername() => _username;
    public int? GetRolId() => _rolId;

    public void SetUser(int usuarioId, string username, int rolId)
    {
        _usuarioId = usuarioId;
        _username = username;
        _rolId = rolId;
    }

    public void Clear()
    {
        _usuarioId = null;
        _username = null;
        _rolId = null;
    }
}