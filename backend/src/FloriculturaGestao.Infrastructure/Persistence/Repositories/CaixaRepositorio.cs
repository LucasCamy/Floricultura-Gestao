using FloriculturaGestao.Domain.Caixa;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class CaixaRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Caixa>(context), ICaixaRepositorio
{
    public async Task<Caixa?> ObterCaixaAbertoAsync(CancellationToken ct = default)
        => await DbSet.Include(c => c.Movimentacoes)
            .FirstOrDefaultAsync(c => c.Status == StatusCaixa.Aberto, ct);

    public async Task<IReadOnlyList<Caixa>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken ct = default)
        => await DbSet.Include(c => c.Movimentacoes)
            .Where(c => c.DataAbertura >= inicio && c.DataAbertura <= fim)
            .AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<Caixa>> ObterTodosComMovimentacoesAsync(CancellationToken ct = default)
        => await DbSet.Include(c => c.Movimentacoes)
            .AsNoTracking().ToListAsync(ct);
}
