namespace FloriculturaGestao.Application.Usuarios.DTOs;

public record UsuarioDto(
    Guid Id,
    string Nome,
    string Email,
    string Perfil,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
