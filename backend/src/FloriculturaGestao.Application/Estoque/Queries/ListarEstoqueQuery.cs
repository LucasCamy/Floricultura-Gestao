using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Application.Estoque.DTOs;
using FloriculturaGestao.Domain.Estoque;
using MediatR;

namespace FloriculturaGestao.Application.Estoque.Queries;

public record ListarEstoqueQuery : IRequest<ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>>;

public class ListarEstoqueHandler(IEstoqueRepositorio repositorio)
    : IRequestHandler<ListarEstoqueQuery, ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>> Handle(ListarEstoqueQuery request, CancellationToken ct)
    {
        var itens = await repositorio.ObterTodosAsync(ct);
        var dtos = itens.Select(i => new EstoqueItemDto(
            i.Id, i.ProdutoId, i.Quantidade, i.EstoqueMinimo,
            i.DataValidade, i.Localizacao, i.EstaBaixo, i.EstaVencido
        )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>.Ok(dtos);
    }
}

public record ListarEstoqueBaixoQuery : IRequest<ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>>;

public class ListarEstoqueBaixoHandler(IEstoqueRepositorio repositorio)
    : IRequestHandler<ListarEstoqueBaixoQuery, ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>> Handle(ListarEstoqueBaixoQuery request, CancellationToken ct)
    {
        var itens = await repositorio.ObterEstoqueBaixoAsync(ct);
        var dtos = itens.Select(i => new EstoqueItemDto(
            i.Id, i.ProdutoId, i.Quantidade, i.EstoqueMinimo,
            i.DataValidade, i.Localizacao, i.EstaBaixo, i.EstaVencido
        )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<EstoqueItemDto>>.Ok(dtos);
    }
}
