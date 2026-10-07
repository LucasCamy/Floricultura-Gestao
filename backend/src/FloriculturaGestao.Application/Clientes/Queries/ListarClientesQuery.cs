using FloriculturaGestao.Application.Clientes.DTOs;
using FloriculturaGestao.Application.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Queries;

public record ListarClientesQuery : IRequest<ResultadoOperacao<IReadOnlyList<ClienteDto>>>;
