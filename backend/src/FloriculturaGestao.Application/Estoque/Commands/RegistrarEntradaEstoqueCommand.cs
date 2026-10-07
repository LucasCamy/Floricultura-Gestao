using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Estoque;
using MediatR;

namespace FloriculturaGestao.Application.Estoque.Commands;

public record RegistrarEntradaEstoqueCommand(
    Guid ProdutoId,
    int Quantidade,
    int EstoqueMinimo,
    DateTime? DataValidade,
    string? Localizacao,
    string? Observacao
) : IRequest<ResultadoOperacao<Guid>>;

public class RegistrarEntradaEstoqueHandler(IEstoqueRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<RegistrarEntradaEstoqueCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(RegistrarEntradaEstoqueCommand request, CancellationToken ct)
    {
        var existente = await repositorio.ObterPorProdutoIdAsync(request.ProdutoId, ct);

        if (existente is not null)
        {
            existente.RegistrarEntrada(request.Quantidade, request.Observacao);
            repositorio.Atualizar(existente);
            await uow.SaveChangesAsync(ct);
            return ResultadoOperacao<Guid>.Ok(existente.Id, "Entrada de estoque registrada.");
        }

        var item = EstoqueItem.Criar(request.ProdutoId, request.Quantidade, request.EstoqueMinimo, request.DataValidade, request.Localizacao);
        await repositorio.AdicionarAsync(item, ct);
        await uow.SaveChangesAsync(ct);

        return ResultadoOperacao<Guid>.Ok(item.Id, "Item de estoque criado.");
    }
}
