using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Usuarios;
using MediatR;

namespace FloriculturaGestao.Application.Usuarios.Commands;

public record AtualizarUsuarioCommand(
    Guid Id,
    string Nome,
    string Email,
    PerfilUsuario Perfil
) : IRequest<ResultadoOperacao>;

public class AtualizarUsuarioHandler(IUsuarioRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<AtualizarUsuarioCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await repositorio.ObterPorIdAsync(request.Id, cancellationToken);
        if (usuario is null)
            return ResultadoOperacao.Falha("Usuário não encontrado.");

        var emailExistente = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken);
        if (emailExistente is not null && emailExistente.Id != request.Id)
            return ResultadoOperacao.Falha("Este e-mail já está em uso por outro usuário.");

        usuario.Atualizar(request.Nome, request.Email, request.Perfil);
        repositorio.Atualizar(usuario);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao.Ok("Usuário atualizado com sucesso.");
    }
}
