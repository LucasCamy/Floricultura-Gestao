namespace FloriculturaGestao.Application.Caixa.DTOs;

public record CaixaDto(
    Guid Id,
    DateTime DataAbertura,
    DateTime? DataFechamento,
    decimal SaldoInicial,
    decimal SaldoFinal,
    string Status,
    string OperadorId,
    int TotalMovimentacoes
);
