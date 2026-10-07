using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Vendas;

public interface IVendaRepositorio : IRepositorio<Venda>
{
    Task<IReadOnlyList<Venda>> ObterPorClienteAsync(Guid clienteId, CancellationToken ct = default);
    Task<IReadOnlyList<Venda>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken ct = default);
    Task<IReadOnlyList<Venda>> ObterPorCaixaAsync(Guid caixaId, CancellationToken ct = default);
}
