using FloriculturaGestao.Application.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Commands;

public record CriarClienteCommand(
    string Nome,
    string Email,
    string Telefone,
    string? Cpf,
    string? Endereco
) : IRequest<ResultadoOperacao<Guid>>;
