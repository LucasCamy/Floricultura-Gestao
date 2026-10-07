using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Application.Contas.DTOs;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Contas;
using FloriculturaGestao.Domain.Fornecedores;
using MediatR;

namespace FloriculturaGestao.Application.Contas.Queries;

public record ListarContasPagarQuery : IRequest<ResultadoOperacao<IReadOnlyList<ContaPagarDto>>>;

public class ListarContasPagarHandler(IContaPagarRepositorio repo, IFornecedorRepositorio fornecedorRepo)
    : IRequestHandler<ListarContasPagarQuery, ResultadoOperacao<IReadOnlyList<ContaPagarDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<ContaPagarDto>>> Handle(ListarContasPagarQuery request, CancellationToken ct)
    {
        var contas = await repo.ObterTodosAsync(ct);
        var fornecedores = await fornecedorRepo.ObterTodosAsync(ct);
        var lookup = fornecedores.ToDictionary(f => f.Id, f => f.NomeFantasia);

        var dtos = contas
            .OrderByDescending(c => c.DataVencimento)
            .Select(c => new ContaPagarDto(
                c.Id, c.Descricao, c.Valor, c.DataVencimento, c.DataPagamento,
                c.FornecedorId, lookup.GetValueOrDefault(c.FornecedorId, ""),
                c.Status.ToString()
            )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<ContaPagarDto>>.Ok(dtos);
    }
}

public record ListarContasReceberQuery : IRequest<ResultadoOperacao<IReadOnlyList<ContaReceberDto>>>;

public class ListarContasReceberHandler(IContaReceberRepositorio repo, IClienteRepositorio clienteRepo)
    : IRequestHandler<ListarContasReceberQuery, ResultadoOperacao<IReadOnlyList<ContaReceberDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<ContaReceberDto>>> Handle(ListarContasReceberQuery request, CancellationToken ct)
    {
        var contas = await repo.ObterTodosAsync(ct);
        var clientes = await clienteRepo.ObterTodosAsync(ct);
        var lookup = clientes.ToDictionary(c => c.Id, c => c.Nome);

        var dtos = contas
            .OrderByDescending(c => c.DataVencimento)
            .Select(c => new ContaReceberDto(
                c.Id, c.Descricao, c.Valor, c.DataVencimento, c.DataRecebimento,
                c.ClienteId, lookup.GetValueOrDefault(c.ClienteId, ""),
                c.Status.ToString()
            )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<ContaReceberDto>>.Ok(dtos);
    }
}
