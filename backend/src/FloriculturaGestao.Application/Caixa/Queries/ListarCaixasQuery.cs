using FloriculturaGestao.Application.Caixa.DTOs;
using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Caixa;
using MediatR;

namespace FloriculturaGestao.Application.Caixa.Queries;

public record ListarCaixasQuery : IRequest<ResultadoOperacao<IReadOnlyList<CaixaDto>>>;

public class ListarCaixasHandler(ICaixaRepositorio repositorio)
    : IRequestHandler<ListarCaixasQuery, ResultadoOperacao<IReadOnlyList<CaixaDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<CaixaDto>>> Handle(ListarCaixasQuery request, CancellationToken ct)
    {
        var caixas = await repositorio.ObterTodosComMovimentacoesAsync(ct);

        var dtos = caixas
            .OrderByDescending(c => c.DataAbertura)
            .Select(c => new CaixaDto(
                c.Id, c.DataAbertura, c.DataFechamento, c.SaldoInicial, c.SaldoFinal,
                c.Status.ToString(), c.OperadorId, c.Movimentacoes.Count
            )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<CaixaDto>>.Ok(dtos);
    }
}
