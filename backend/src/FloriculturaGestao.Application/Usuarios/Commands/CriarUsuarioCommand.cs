using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Usuarios;
using MediatR;

namespace FloriculturaGestao.Application.Usuarios.Commands;

public record CriarUsuarioCommand(
    string Nome,
    string Email,
    string Senha,
    PerfilUsuario Perfil
) : IRequest<ResultadoOperacao<Guid>>;

public class CriarUsuarioHandler(IUsuarioRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<CriarUsuarioCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
    {
        var existente = await repositorio.ObterPorEmailAsync(request.Email, cancellationToken);
        if (existente is not null)
            return ResultadoOperacao<Guid>.Falha("Já existe um usuário com este e-mail.");

        var usuario = Usuario.Criar(request.Nome, request.Email, request.Senha, request.Perfil);
        await repositorio.AdicionarAsync(usuario, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultadoOperacao<Guid>.Ok(usuario.Id, "Usuário criado com sucesso.");
    }
}
