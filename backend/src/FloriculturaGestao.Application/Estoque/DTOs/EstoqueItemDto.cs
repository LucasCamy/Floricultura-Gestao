namespace FloriculturaGestao.Application.Estoque.DTOs;

public record EstoqueItemDto(
    Guid Id,
    Guid ProdutoId,
    int Quantidade,
    int EstoqueMinimo,
    DateTime? DataValidade,
    string? Localizacao,
    bool EstaBaixo,
    bool EstaVencido
);
