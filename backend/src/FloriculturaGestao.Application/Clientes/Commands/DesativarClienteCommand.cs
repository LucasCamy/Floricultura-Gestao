using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Vendas;
using FloriculturaGestao.Domain.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Commands;

public record DesativarClienteCommand(Guid Id) : IRequest<ResultadoOperacao>;

public class DesativarClienteHandler(IClienteRepositorio repositorio, IVendaRepositorio vendaRepositorio, IUnitOfWork uow)
    : IRequestHandler<DesativarClienteCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(DesativarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (cliente is null)
            return ResultadoOperacao.Falha("Cliente não encontrado.");
        // Consultar repositório de vendas para verificar se o cliente possui vendas.
        var vendasDoCliente = await vendaRepositorio.ObterPorClienteAsync(cliente.Id, cancellationToken);
        if (vendasDoCliente != null && vendasDoCliente.Count > 0)
        {
            cliente.Desativar();
            repositorio.Atualizar(cliente);
            await uow.SaveChangesAsync(cancellationToken);
            return ResultadoOperacao.Ok("Cliente desativado (soft delete) com sucesso.");
        }

        // Sem vendas registradas: remoção física.
        repositorio.Remover(cliente);
        await uow.SaveChangesAsync(cancellationToken);
        return ResultadoOperacao.Ok("Cliente removido com sucesso.");
    }
}