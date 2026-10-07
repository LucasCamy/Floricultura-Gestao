using FloriculturaGestao.Application.Clientes.DTOs;
using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Clientes;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Queries;

public class ObterClientePorIdHandler(IClienteRepositorio repositorio)
    : IRequestHandler<ObterClientePorIdQuery, ResultadoOperacao<ClienteDto>>
{
    public async Task<ResultadoOperacao<ClienteDto>> Handle(ObterClientePorIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (cliente is null)
            return ResultadoOperacao<ClienteDto>.Falha("Cliente não encontrado.");

        var dto = new ClienteDto(
            cliente.Id, cliente.Nome, cliente.Email, cliente.Telefone,
            cliente.Cpf, cliente.Endereco, cliente.Observacoes, cliente.Ativo, cliente.CriadoEm
        );
        return ResultadoOperacao<ClienteDto>.Ok(dto);
    }
}
