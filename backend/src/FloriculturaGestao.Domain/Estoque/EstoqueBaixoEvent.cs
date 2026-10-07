using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Estoque;

public class EstoqueBaixoEvent : EventoDominio
{
    public Guid EstoqueItemId { get; }
    public Guid ProdutoId { get; }
    public int QuantidadeAtual { get; }
    public int EstoqueMinimo { get; }

    public EstoqueBaixoEvent(Guid estoqueItemId, Guid produtoId, int quantidadeAtual, int estoqueMinimo)
    {
        EstoqueItemId = estoqueItemId;
        ProdutoId = produtoId;
        QuantidadeAtual = quantidadeAtual;
        EstoqueMinimo = estoqueMinimo;
    }
}
