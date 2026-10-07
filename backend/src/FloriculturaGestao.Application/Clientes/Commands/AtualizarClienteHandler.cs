using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Commands;

public class AtualizarClienteHandler(IClienteRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<AtualizarClienteCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(AtualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (cliente is null)
            return ResultadoOperacao.Falha("Cliente não encontrado.");

        cliente.Atualizar(request.Nome, request.Email, request.Telefone, request.Cpf, request.Endereco, request.Observacoes);
        repositorio.Atualizar(cliente);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao.Ok("Cliente atualizado com sucesso.");
    }
}
