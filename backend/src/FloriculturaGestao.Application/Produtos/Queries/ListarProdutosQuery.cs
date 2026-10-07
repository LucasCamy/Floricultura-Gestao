using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Application.Produtos.DTOs;
using FloriculturaGestao.Domain.Produtos;
using MediatR;

namespace FloriculturaGestao.Application.Produtos.Queries;

public record ListarProdutosQuery : IRequest<ResultadoOperacao<IReadOnlyList<ProdutoDto>>>;

public class ListarProdutosHandler(IProdutoRepositorio repositorio)
    : IRequestHandler<ListarProdutosQuery, ResultadoOperacao<IReadOnlyList<ProdutoDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<ProdutoDto>>> Handle(ListarProdutosQuery request, CancellationToken ct)
    {
        var produtos = await repositorio.ObterTodosAsync(ct);
        var dtos = produtos.Select(p => new ProdutoDto(
            p.Id, p.Nome, p.Descricao, p.Sku, p.Categoria.ToString(),
            p.PrecoCompra, p.PrecoVenda, p.UnidadeMedida, p.Ativo, p.CriadoEm
        )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<ProdutoDto>>.Ok(dtos);
    }
}
