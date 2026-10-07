using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Produtos;

public interface IProdutoRepositorio : IRepositorio<Produto>
{
    Task<Produto?> ObterPorSkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Produto>> BuscarPorCategoriaAsync(CategoriaProduto categoria, CancellationToken ct = default);
}
