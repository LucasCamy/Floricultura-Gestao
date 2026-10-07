using FloriculturaGestao.Domain.Estoque;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class EstoqueRepositorio(FloriculturaDbContext context)
    : RepositorioBase<EstoqueItem>(context), IEstoqueRepositorio
{
    public async Task<EstoqueItem?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken ct = default)
        => await DbSet.Include(e => e.Movimentacoes).FirstOrDefaultAsync(e => e.ProdutoId == produtoId, ct);

    public async Task<IReadOnlyList<EstoqueItem>> ObterEstoqueBaixoAsync(CancellationToken ct = default)
        => await DbSet.Where(e => e.Quantidade <= e.EstoqueMinimo).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<EstoqueItem>> ObterProdutosVencidosAsync(CancellationToken ct = default)
        => await DbSet.Where(e => e.DataValidade != null && e.DataValidade.Value.Date <= DateTime.UtcNow.Date)
            .AsNoTracking().ToListAsync(ct);
}
