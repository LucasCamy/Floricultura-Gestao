using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Produtos;
using MediatR;

namespace FloriculturaGestao.Application.Produtos.Commands;

public record CriarProdutoCommand(
    string Nome,
    string Sku,
    string Categoria,
    decimal PrecoCompra,
    decimal PrecoVenda,
    string? Descricao,
    string? UnidadeMedida
) : IRequest<ResultadoOperacao<Guid>>;

public class CriarProdutoHandler(IProdutoRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<CriarProdutoCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(CriarProdutoCommand request, CancellationToken ct)
    {
        var existente = await repositorio.ObterPorSkuAsync(request.Sku, ct);
        if (existente is not null)
            return ResultadoOperacao<Guid>.Falha("Já existe um produto com este SKU.");

        if (!Enum.TryParse<CategoriaProduto>(request.Categoria, true, out var categoria))
            return ResultadoOperacao<Guid>.Falha("Categoria inválida.");

        var produto = Produto.Criar(request.Nome, request.Sku, categoria, request.PrecoCompra, request.PrecoVenda, request.Descricao, request.UnidadeMedida);
        await repositorio.AdicionarAsync(produto, ct);
        await uow.SaveChangesAsync(ct);

        return ResultadoOperacao<Guid>.Ok(produto.Id, "Produto criado com sucesso.");
    }
}
