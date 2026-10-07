using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Usuarios;
using MediatR;

namespace FloriculturaGestao.Application.Usuarios.Commands;

public record DesativarUsuarioCommand(Guid Id) : IRequest<ResultadoOperacao>;

public class DesativarUsuarioHandler(IUsuarioRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<DesativarUsuarioCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(DesativarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");

        if (!usuario.Ativo)
            return ResultadoOperacao.Falha("Usuário já está inativo.");

        usuario.Desativar();
        repositorio.Atualizar(usuario);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao.Ok("Usuário desativado com sucesso.");
    }
}

public record AtivarUsuarioCommand(Guid Id) : IRequest<ResultadoOperacao>;

public class AtivarUsuarioHandler(IUsuarioRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<AtivarUsuarioCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(AtivarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");

        usuario.Ativar();
        repositorio.Atualizar(usuario);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao.Ok("Usuário ativado com sucesso.");
    }
}
