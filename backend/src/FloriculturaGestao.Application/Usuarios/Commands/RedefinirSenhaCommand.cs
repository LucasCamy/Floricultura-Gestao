using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Usuarios;
using MediatR;

namespace FloriculturaGestao.Application.Usuarios.Commands;

public record RedefinirSenhaCommand(Guid Id, string NovaSenha) : IRequest<ResultadoOperacao>;

public class RedefinirSenhaHandler(IUsuarioRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<RedefinirSenhaCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(RedefinirSenhaCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NovaSenha) || request.NovaSenha.Length < 6)
            return ResultadoOperacao.Falha("A senha deve ter pelo menos 6 caracteres.");

        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");

        usuario.RedefinirSenha(request.NovaSenha);
        repositorio.Atualizar(usuario);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao.Ok("Senha redefinida com sucesso.");
    }
}
