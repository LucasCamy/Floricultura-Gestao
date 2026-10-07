using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Usuarios;

public interface IUsuarioRepositorio : IRepositorio<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExisteAdminAsync(CancellationToken ct = default);
}
