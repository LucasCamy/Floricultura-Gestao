namespace FloriculturaGestao.Application.Contas.DTOs;

public record ContaPagarDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateTime DataVencimento,
    DateTime? DataPagamento,
    Guid FornecedorId,
    string FornecedorNome,
    string Status
);

public record ContaReceberDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    DateTime DataVencimento,
    DateTime? DataRecebimento,
    Guid ClienteId,
    string ClienteNome,
    string Status
);
