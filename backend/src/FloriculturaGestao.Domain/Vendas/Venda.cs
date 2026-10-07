using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Vendas;

public enum StatusVenda
{
    Aberta,
    Finalizada,
    Cancelada
}

public class Venda : EntidadeBase
{
    public Guid ClienteId { get; private set; }
    public Guid? CaixaId { get; private set; }
    public StatusVenda Status { get; private set; } = StatusVenda.Aberta;
    public decimal ValorTotal { get; private set; }
    public decimal Desconto { get; private set; }
    public string? Observacoes { get; private set; }

    private readonly List<ItemVenda> _itens = [];
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    private Venda() { }

    public static Venda Criar(Guid clienteId, Guid? caixaId = null)
    {
        return new Venda
        {
            ClienteId = clienteId,
            CaixaId = caixaId
        };
    }

    public void AdicionarItem(Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (Status != StatusVenda.Aberta)
            throw new InvalidOperationException("Não é possível adicionar itens a uma venda que não está aberta.");

        var item = ItemVenda.Criar(Id, produtoId, nomeProduto, quantidade, precoUnitario);
        _itens.Add(item);
        RecalcularTotal();
    }

    public void RemoverItem(Guid itemId)
    {
        if (Status != StatusVenda.Aberta)
            throw new InvalidOperationException("Não é possível remover itens de uma venda que não está aberta.");

        var item = _itens.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidOperationException("Item não encontrado na venda.");
        _itens.Remove(item);
        RecalcularTotal();
    }

    public void AplicarDesconto(decimal desconto)
    {
        if (desconto < 0 || desconto > ValorTotal)
            throw new ArgumentException("Desconto inválido.");
        Desconto = desconto;
        RecalcularTotal();
    }

    public void Finalizar()
    {
        if (_itens.Count == 0)
            throw new InvalidOperationException("Venda sem itens não pode ser finalizada.");
        Status = StatusVenda.Finalizada;
        AdicionarEvento(new VendaRealizadaEvent(Id, ClienteId, ValorTotal));
        MarcarAtualizado();
    }

    public void Cancelar()
    {
        Status = StatusVenda.Cancelada;
        MarcarAtualizado();
    }

    private void RecalcularTotal()
    {
        ValorTotal = _itens.Sum(i => i.Subtotal) - Desconto;
        if (ValorTotal < 0) ValorTotal = 0;
        MarcarAtualizado();
    }
}

public class ItemVenda : EntidadeBase
{
    public Guid VendaId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemVenda() { }

    public static ItemVenda Criar(Guid vendaId, Guid produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser positiva.");
        if (precoUnitario < 0) throw new ArgumentException("Preço unitário não pode ser negativo.");

        return new ItemVenda
        {
            VendaId = vendaId,
            ProdutoId = produtoId,
            NomeProduto = nomeProduto,
            Quantidade = quantidade,
            PrecoUnitario = precoUnitario
        };
    }
}
