using FloriculturaGestao.Domain.Fornecedores;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public class FornecedorRepositorio(FloriculturaDbContext context)
    : RepositorioBase<Fornecedor>(context), IFornecedorRepositorio
{
    public async Task<Fornecedor?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default)
        => await DbSet.FirstOrDefaultAsync(f => f.Cnpj == cnpj, ct);
}
