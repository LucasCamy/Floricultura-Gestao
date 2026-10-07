using FloriculturaGestao.Application.Clientes.DTOs;
using FloriculturaGestao.Application.Common;
using MediatR;

namespace FloriculturaGestao.Application.Clientes.Queries;

public record ObterClientePorIdQuery(Guid Id) : IRequest<ResultadoOperacao<ClienteDto>>;
