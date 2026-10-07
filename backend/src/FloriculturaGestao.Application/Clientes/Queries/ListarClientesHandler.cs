using FloriculturaGestao.Application.Clientes.DTOs;
using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Clientes;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Queries;

public class ListarClientesHandler(IClienteRepositorio repositorio)
    : IRequestHandler<ListarClientesQuery, ResultadoOperacao<IReadOnlyList<ClienteDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<ClienteDto>>> Handle(ListarClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await repositorio.ObterTodosAsync(cancellationToken);
        var dtos = clientes.Select(c => new ClienteDto(
            c.Id, c.Nome, c.Email, c.Telefone, c.Cpf, c.Endereco, c.Observacoes, c.Ativo, c.CriadoEm
        )).ToList().AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<ClienteDto>>.Ok(dtos);
    }
}
