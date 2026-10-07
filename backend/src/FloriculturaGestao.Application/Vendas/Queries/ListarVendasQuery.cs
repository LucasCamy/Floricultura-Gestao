using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Application.Vendas.DTOs;
using FloriculturaGestao.Domain.Vendas;
using MediatR;

namespace FloriculturaGestao.Application.Vendas.Queries;

public record ListarVendasPorPeriodoQuery(DateTime Inicio, DateTime Fim) : IRequest<ResultadoOperacao<IReadOnlyList<VendaDto>>>;

public class ListarVendasPorPeriodoHandler(IVendaRepositorio repositorio)
    : IRequestHandler<ListarVendasPorPeriodoQuery, ResultadoOperacao<IReadOnlyList<VendaDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<VendaDto>>> Handle(ListarVendasPorPeriodoQuery request, CancellationToken ct)
    {
        var vendas = await repositorio.ObterPorPeriodoAsync(request.Inicio, request.Fim, ct);
        var dtos = vendas.Select(MapToDto).ToList().AsReadOnly();
        return ResultadoOperacao<IReadOnlyList<VendaDto>>.Ok(dtos);
    }

    private static VendaDto MapToDto(Venda v) => new(
        v.Id, v.ClienteId, v.CaixaId, v.Status.ToString(), v.ValorTotal, v.Desconto,
        v.Observacoes, v.CriadoEm,
        v.Itens.Select(i => new ItemVendaDto(i.Id, i.ProdutoId, i.NomeProduto, i.Quantidade, i.PrecoUnitario, i.Subtotal)).ToList()
    );
}
