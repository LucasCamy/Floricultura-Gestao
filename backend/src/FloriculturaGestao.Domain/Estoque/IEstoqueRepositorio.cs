using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Estoque;

public interface IEstoqueRepositorio : IRepositorio<EstoqueItem>
{
    Task<EstoqueItem?> ObterPorProdutoIdAsync(Guid produtoId, CancellationToken ct = default);
    Task<IReadOnlyList<EstoqueItem>> ObterEstoqueBaixoAsync(CancellationToken ct = default);
    Task<IReadOnlyList<EstoqueItem>> ObterProdutosVencidosAsync(CancellationToken ct = default);
}
