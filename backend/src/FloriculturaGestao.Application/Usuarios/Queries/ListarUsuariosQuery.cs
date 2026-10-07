using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Application.Usuarios.DTOs;
using FloriculturaGestao.Domain.Usuarios;
using MediatR;

namespace FloriculturaGestao.Application.Usuarios.Queries;

public record ListarUsuariosQuery : IRequest<ResultadoOperacao<IReadOnlyList<UsuarioDto>>>;

public class ListarUsuariosHandler(IUsuarioRepositorio repositorio)
    : IRequestHandler<ListarUsuariosQuery, ResultadoOperacao<IReadOnlyList<UsuarioDto>>>
{
    public async Task<ResultadoOperacao<IReadOnlyList<UsuarioDto>>> Handle(ListarUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await repositorio.ObterTodosAsync(cancellationToken);
        var dtos = usuarios
            .Select(u => new UsuarioDto(u.Id, u.Nome, u.Email, u.Perfil.ToString(), u.Ativo, u.CriadoEm, u.AtualizadoEm))
            .ToList()
            .AsReadOnly();

        return ResultadoOperacao<IReadOnlyList<UsuarioDto>>.Ok(dtos);
    }
}
