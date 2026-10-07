using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Clientes;

public interface IClienteRepositorio : IRepositorio<Cliente>
{
    Task<IReadOnlyList<Cliente>> BuscarPorNomeAsync(string nome, CancellationToken ct = default);
    Task<Cliente?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken ct = default);
}
