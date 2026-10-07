using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Caixa;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Estoque;
using FloriculturaGestao.Domain.Produtos;
using FloriculturaGestao.Domain.Vendas;
using MediatR;

namespace FloriculturaGestao.Application.Vendas.Commands;

public class RegistrarVendaHandler(
    IVendaRepositorio vendaRepo,
    IProdutoRepositorio produtoRepo,
    IEstoqueRepositorio estoqueRepo,
    ICaixaRepositorio caixaRepo,
    IUnitOfWork uow,
    IMediator mediator)
    : IRequestHandler<RegistrarVendaCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(RegistrarVendaCommand request, CancellationToken ct)
    {
        if (request.Itens.Count == 0)
            return ResultadoOperacao<Guid>.Falha("A venda deve ter pelo menos um item.");

        var venda = Venda.Criar(request.ClienteId, request.CaixaId);

        foreach (var item in request.Itens)
        {
            var produto = await produtoRepo.ObterPorIdAsync(item.ProdutoId, ct);
            if (produto is null)
                return ResultadoOperacao<Guid>.Falha($"Produto {item.ProdutoId} não encontrado.");

            var estoque = await estoqueRepo.ObterPorProdutoIdAsync(item.ProdutoId, ct);
            if (estoque is null || estoque.Quantidade < item.Quantidade)
                return ResultadoOperacao<Guid>.Falha($"Estoque insuficiente para o produto '{produto.Nome}'.");

            venda.AdicionarItem(produto.Id, produto.Nome, item.Quantidade, produto.PrecoVenda);
            estoque.RegistrarSaida(item.Quantidade, $"Venda {venda.Id}");
        }

        if (request.Desconto > 0)
            venda.AplicarDesconto(request.Desconto);

        venda.Finalizar();

        // Registrar no caixa se informado
        if (request.CaixaId.HasValue)
        {
            var caixa = await caixaRepo.ObterPorIdAsync(request.CaixaId.Value, ct);
            caixa?.RegistrarMovimentacao(TipoMovimentacaoCaixa.Venda, venda.ValorTotal, $"Venda {venda.Id}", venda.Id);
        }

        await vendaRepo.AdicionarAsync(venda, ct);
        await uow.SaveChangesAsync(ct);

        // Publicar eventos de domínio
        foreach (var evento in venda.Eventos)
            await mediator.Publish(evento, ct);

        return ResultadoOperacao<Guid>.Ok(venda.Id, "Venda registrada com sucesso.");
    }
}
