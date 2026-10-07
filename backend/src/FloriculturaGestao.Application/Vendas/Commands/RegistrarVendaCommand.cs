using FloriculturaGestao.Application.Common;
using MediatR;

namespace FloriculturaGestao.Application.Vendas.Commands;

public record ItemVendaInput(Guid ProdutoId, int Quantidade);

public record RegistrarVendaCommand(
    Guid ClienteId,
    Guid? CaixaId,
    List<ItemVendaInput> Itens,
    decimal Desconto = 0
) : IRequest<ResultadoOperacao<Guid>>;
