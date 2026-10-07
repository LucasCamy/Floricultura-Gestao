namespace FloriculturaGestao.Application.Clientes.DTOs;

public record ClienteDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? Cpf,
    string? Endereco,
    string? Observacoes,
    bool Ativo,
    DateTime CriadoEm
);
