using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Commands;

public class CriarClienteHandler(IClienteRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<CriarClienteCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var existente = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken);
        if (existente is not null)
            return ResultadoOperacao<Guid>.Falha("Já existe um cliente com este email.");

        if (!string.IsNullOrWhiteSpace(request.Cpf))
        {
            var porCpf = await repositorio.ObterPorCpfAsync(request.Cpf, cancellationToken);
            if (porCpf is not null)
                return ResultadoOperacao<Guid>.Falha("Já existe um cliente com este CPF.");
        }

        var cliente = Cliente.Criar(request.Nome, request.Email, request.Telefone, request.Cpf, request.Endereco);
        await repositorio.AdicionarAsync(cliente, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao<Guid>.Ok(cliente.Id, "Cliente criado com sucesso.");
    }
}
