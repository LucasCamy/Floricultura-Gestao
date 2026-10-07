using FloriculturaGestao.Domain.Contas;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class ContaPagarRepositorio(FloriculturaDbContext context)
    : RepositorioBase<ContaPagar>(context), IContaPagarRepositorio
{
    public async Task<IReadOnlyList<ContaPagar>> ObterPorFornecedorAsync(Guid fornecedorId, CancellationToken ct = default)
        => await DbSet.Where(c => c.FornecedorId == fornecedorId).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<ContaPagar>> ObterPendentesAsync(CancellationToken ct = default)
        => await DbSet.Where(c => c.Status == StatusConta.Pendente).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<ContaPagar>> ObterVencidasAsync(CancellationToken ct = default)
        => await DbSet.Where(c => c.Status == StatusConta.Vencida).AsNoTracking().ToListAsync(ct);
}

public class ContaReceberRepositorio(FloriculturaDbContext context)
    : RepositorioBase<ContaReceber>(context), IContaReceberRepositorio
{
    public async Task<IReadOnlyList<ContaReceber>> ObterPorClienteAsync(Guid clienteId, CancellationToken ct = default)
        => await DbSet.Where(c => c.ClienteId == clienteId).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<ContaReceber>> ObterPendentesAsync(CancellationToken ct = default)
        => await DbSet.Where(c => c.Status == StatusConta.Pendente).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<ContaReceber>> ObterVencidasAsync(CancellationToken ct = default)
        => await DbSet.Where(c => c.Status == StatusConta.Vencida).AsNoTracking().ToListAsync(ct);
}
