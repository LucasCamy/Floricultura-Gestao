using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Fornecedores;

public interface IFornecedorRepositorio : IRepositorio<Fornecedor>
{
    Task<Fornecedor?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default);
}
