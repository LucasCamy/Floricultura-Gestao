using FloriculturaGestao.Domain.Vendas;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class VendaRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Venda>(context), IVendaRepositorio
{
    public new async Task<Venda?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.Include(v => v.Itens).FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<IReadOnlyList<Venda>> ObterPorClienteAsync(Guid clienteId, CancellationToken ct = default)
        => await DbSet.Include(v => v.Itens).Where(v => v.ClienteId == clienteId).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<Venda>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken ct = default)
        => await DbSet.Include(v => v.Itens).Where(v => v.CriadoEm >= inicio && v.CriadoEm <= fim).AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<Venda>> ObterPorCaixaAsync(Guid caixaId, CancellationToken ct = default)
        => await DbSet.Include(v => v.Itens).Where(v => v.CaixaId == caixaId).AsNoTracking().ToListAsync(ct);
}
