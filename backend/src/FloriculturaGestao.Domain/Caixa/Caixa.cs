using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Caixa;

public enum StatusCaixa
{
    Aberto,
    Fechado
}

public enum TipoMovimentacaoCaixa
{
    Venda,
    Pagamento,
    Recebimento,
    Sangria,
    Suprimento
}

public class Caixa : EntidadeBase
{
    public DateTime DataAbertura { get; private set; }
    public DateTime? DataFechamento { get; private set; }
    public decimal SaldoInicial { get; private set; }
    public decimal SaldoFinal { get; private set; }
    public StatusCaixa Status { get; private set; } = StatusCaixa.Aberto;
    public string OperadorId { get; private set; } = string.Empty;

    private readonly List<MovimentacaoCaixa> _movimentacoes = [];
    public IReadOnlyCollection<MovimentacaoCaixa> Movimentacoes => _movimentacoes.AsReadOnly();

    private Caixa() { }

    public static Caixa Abrir(decimal saldoInicial, string operadorId)
    {
        if (saldoInicial < 0) throw new ArgumentException("Saldo inicial não pode ser negativo.");
        ArgumentException.ThrowIfNullOrWhiteSpace(operadorId);

        return new Caixa
        {
            DataAbertura = DateTime.UtcNow,
            SaldoInicial = saldoInicial,
            SaldoFinal = saldoInicial,
            OperadorId = operadorId
        };
    }

    public void RegistrarMovimentacao(TipoMovimentacaoCaixa tipo, decimal valor, string descricao, Guid? referenciaId = null)
    {
        if (Status != StatusCaixa.Aberto)
            throw new InvalidOperationException("Caixa está fechado.");
        if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");

        var mov = MovimentacaoCaixa.Criar(Id, tipo, valor, descricao, referenciaId);
        _movimentacoes.Add(mov);

        switch (tipo)
        {
            case TipoMovimentacaoCaixa.Venda:
            case TipoMovimentacaoCaixa.Recebimento:
            case TipoMovimentacaoCaixa.Suprimento:
                SaldoFinal += valor;
                break;
            case TipoMovimentacaoCaixa.Pagamento:
            case TipoMovimentacaoCaixa.Sangria:
                SaldoFinal -= valor;
                break;
        }

        MarcarAtualizado();
    }

    public void Fechar()
    {
        if (Status != StatusCaixa.Aberto)
            throw new InvalidOperationException("Caixa já está fechado.");

        Status = StatusCaixa.Fechado;
        DataFechamento = DateTime.UtcNow;
        MarcarAtualizado();
    }
}

public class MovimentacaoCaixa : EntidadeBase
{
    public Guid CaixaId { get; private set; }
    public TipoMovimentacaoCaixa Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public Guid? ReferenciaId { get; private set; }

    private MovimentacaoCaixa() { }

    public static MovimentacaoCaixa Criar(Guid caixaId, TipoMovimentacaoCaixa tipo, decimal valor, string descricao, Guid? referenciaId = null)
    {
        return new MovimentacaoCaixa
        {
            CaixaId = caixaId,
            Tipo = tipo,
            Valor = valor,
            Descricao = descricao,
            ReferenciaId = referenciaId
        };
    }
}
