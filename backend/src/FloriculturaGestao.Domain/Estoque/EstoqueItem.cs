using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Estoque;

public enum TipoMovimentacao
{
    Entrada,
    Saida,
    Ajuste,
    Perda
}

public class EstoqueItem : EntidadeBase
{
    public Guid ProdutoId { get; private set; }
    public int Quantidade { get; private set; }
    public int EstoqueMinimo { get; private set; }
    public DateTime? DataValidade { get; private set; }
    public string? Localizacao { get; private set; }

    private readonly List<MovimentacaoEstoque> _movimentacoes = [];
    public IReadOnlyCollection<MovimentacaoEstoque> Movimentacoes => _movimentacoes.AsReadOnly();

    private EstoqueItem() { }

    public static EstoqueItem Criar(Guid produtoId, int quantidadeInicial, int estoqueMinimo, DateTime? dataValidade = null, string? localizacao = null)
    {
        if (quantidadeInicial < 0) throw new ArgumentException("Quantidade não pode ser negativa.");

        var item = new EstoqueItem
        {
            ProdutoId = produtoId,
            Quantidade = quantidadeInicial,
            EstoqueMinimo = estoqueMinimo,
            DataValidade = dataValidade,
            Localizacao = localizacao
        };

        item._movimentacoes.Add(MovimentacaoEstoque.Criar(item.Id, TipoMovimentacao.Entrada, quantidadeInicial, "Estoque inicial"));
        return item;
    }

    public void RegistrarEntrada(int quantidade, string? observacao = null)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser positiva.");
        Quantidade += quantidade;
        _movimentacoes.Add(MovimentacaoEstoque.Criar(Id, TipoMovimentacao.Entrada, quantidade, observacao));
        MarcarAtualizado();
    }

    public void RegistrarSaida(int quantidade, string? observacao = null)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser positiva.");
        if (quantidade > Quantidade) throw new InvalidOperationException("Estoque insuficiente.");
        Quantidade -= quantidade;
        _movimentacoes.Add(MovimentacaoEstoque.Criar(Id, TipoMovimentacao.Saida, quantidade, observacao));

        if (EstaBaixo)
            AdicionarEvento(new EstoqueBaixoEvent(Id, ProdutoId, Quantidade, EstoqueMinimo));

        MarcarAtualizado();
    }

    public void RegistrarPerda(int quantidade, string motivo)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser positiva.");
        if (quantidade > Quantidade) throw new InvalidOperationException("Quantidade de perda maior que estoque.");
        Quantidade -= quantidade;
        _movimentacoes.Add(MovimentacaoEstoque.Criar(Id, TipoMovimentacao.Perda, quantidade, motivo));
        MarcarAtualizado();
    }

    public bool EstaBaixo => Quantidade <= EstoqueMinimo;
    public bool EstaVencido => DataValidade.HasValue && DataValidade.Value.Date <= DateTime.UtcNow.Date;
}

public class MovimentacaoEstoque : EntidadeBase
{
    public Guid EstoqueItemId { get; private set; }
    public TipoMovimentacao Tipo { get; private set; }
    public int Quantidade { get; private set; }
    public string? Observacao { get; private set; }

    private MovimentacaoEstoque() { }

    public static MovimentacaoEstoque Criar(Guid estoqueItemId, TipoMovimentacao tipo, int quantidade, string? observacao)
    {
        return new MovimentacaoEstoque
        {
            EstoqueItemId = estoqueItemId,
            Tipo = tipo,
            Quantidade = quantidade,
            Observacao = observacao
        };
    }
}
