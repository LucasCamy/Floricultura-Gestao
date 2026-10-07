namespace FloriculturaGestao.Application.Vendas.DTOs;

public record VendaDto(
    Guid Id,
    Guid ClienteId,
    Guid? CaixaId,
    string Status,
    decimal ValorTotal,
    decimal Desconto,
    string? Observacoes,
    DateTime CriadoEm,
    List<ItemVendaDto> Itens
);

public record ItemVendaDto(
    Guid Id,
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);
