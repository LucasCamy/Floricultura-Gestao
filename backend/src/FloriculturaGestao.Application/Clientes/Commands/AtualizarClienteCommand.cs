using FloriculturaGestao.Application.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Commands;

public record AtualizarClienteCommand(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? Cpf,
    string? Endereco,
    string? Observacoes
) : IRequest<ResultadoOperacao>;
