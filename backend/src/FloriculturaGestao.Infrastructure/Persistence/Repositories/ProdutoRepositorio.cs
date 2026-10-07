using FloriculturaGestao.Domain.Produtos;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class ProdutoRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Produto>(context), IProdutoRepositorio
{
    public async Task<Produto?> ObterPorSkuAsync(string sku, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(p => p.Sku == sku.ToUpperInvariant(), ct);

    public async Task<IReadOnlyList<Produto>> BuscarPorCategoriaAsync(CategoriaProduto categoria, CancellationToken ct = default)
        => await DbSet.Where(p => p.Categoria == categoria).AsNoTracking().ToListAsync(ct);
}
