using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Contas;

public enum StatusConta
{
    Pendente,
    Paga,
    Vencida,
    Cancelada
}

public class ContaPagar : EntidadeBase
{
    public Guid FornecedorId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public StatusConta Status { get; private set; } = StatusConta.Pendente;
    public string? Observacoes { get; private set; }

    private ContaPagar() { }

    public static ContaPagar Criar(Guid fornecedorId, string descricao, decimal valor, DateTime dataVencimento, string? observacoes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);
        if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");

        return new ContaPagar
        {
            FornecedorId = fornecedorId,
            Descricao = descricao.Trim(),
            Valor = valor,
            DataVencimento = dataVencimento,
            Observacoes = observacoes
        };
    }

    public void RegistrarPagamento()
    {
        if (Status != StatusConta.Pendente && Status != StatusConta.Vencida)
            throw new InvalidOperationException("Conta não está pendente.");
        Status = StatusConta.Paga;
        DataPagamento = DateTime.UtcNow;
        MarcarAtualizado();
    }

    public void Cancelar()
    {
        Status = StatusConta.Cancelada;
        MarcarAtualizado();
    }

    public void VerificarVencimento()
    {
        if (Status == StatusConta.Pendente && DataVencimento.Date < DateTime.UtcNow.Date)
        {
            Status = StatusConta.Vencida;
            MarcarAtualizado();
        }
    }
}

public class ContaReceber : EntidadeBase
{
    public Guid ClienteId { get; private set; }
    public Guid? VendaId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime DataVencimento { get; private set; }
    public DateTime? DataRecebimento { get; private set; }
    public StatusConta Status { get; private set; } = StatusConta.Pendente;
    public string? Observacoes { get; private set; }

    private ContaReceber() { }

    public static ContaReceber Criar(Guid clienteId, string descricao, decimal valor, DateTime dataVencimento, Guid? vendaId = null, string? observacoes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);
        if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");

        return new ContaReceber
        {
            ClienteId = clienteId,
            VendaId = vendaId,
            Descricao = descricao.Trim(),
            Valor = valor,
            DataVencimento = dataVencimento,
            Observacoes = observacoes
        };
    }

    public void RegistrarRecebimento()
    {
        if (Status != StatusConta.Pendente && Status != StatusConta.Vencida)
            throw new InvalidOperationException("Conta não está pendente.");
        Status = StatusConta.Paga;
        DataRecebimento = DateTime.UtcNow;
        MarcarAtualizado();
    }

    public void Cancelar()
    {
        Status = StatusConta.Cancelada;
        MarcarAtualizado();
    }

    public void VerificarVencimento()
    {
        if (Status == StatusConta.Pendente && DataVencimento.Date < DateTime.UtcNow.Date)
        {
            Status = StatusConta.Vencida;
            MarcarAtualizado();
        }
    }
}
