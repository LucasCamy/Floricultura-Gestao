using FloriculturaGestao.Domain.Clientes;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class ClienteRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Cliente>(context), IClienteRepositorio
{
    public async Task<IReadOnlyList<Cliente>> BuscarPorNomeAsync(string nome, CancellationToken ct = default)
        => await DbSet.Where(c => EF.Functions.ILike(c.Nome, $"%{nome}%")).AsNoTracking().ToListAsync(ct);

    public async Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);

    public async Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(c => c.Cpf == cpf, ct);
}
