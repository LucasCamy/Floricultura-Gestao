using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Caixa;

public interface ICaixaRepositorio : IRepositorio<Caixa>
{
    Task<Caixa?> ObterCaixaAbertoAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Caixa>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim, CancellationToken ct = default);
    Task<IReadOnlyList<Caixa>> ObterTodosComMovimentacoesAsync(CancellationToken ct = default);
}
