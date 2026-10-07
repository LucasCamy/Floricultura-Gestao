using FloriculturaGestao.Domain.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class UsuarioRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Usuario>(context), IUsuarioRepositorio
{
    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<bool> ExisteAdminAsync(CancellationToken ct = default)
        => await DbSet.AnyAsync(u => u.Perfil == PerfilUsuario.Admin, ct);
}
