using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Contas;

public interface IContaPagarRepositorio : IRepositorio<ContaPagar>
{
    Task<IReadOnlyList<ContaPagar>> ObterPorFornecedorAsync(Guid fornecedorId, CancellationToken ct = default);
    Task<IReadOnlyList<ContaPagar>> ObterPendentesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ContaPagar>> ObterVencidasAsync(CancellationToken ct = default);
}

public interface IContaReceberRepositorio : IRepositorio<ContaReceber>
{
    Task<IReadOnlyList<ContaReceber>> ObterPorClienteAsync(Guid clienteId, CancellationToken ct = default);
    Task<IReadOnlyList<ContaReceber>> ObterPendentesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ContaReceber>> ObterVencidasAsync(CancellationToken ct = default);
}
