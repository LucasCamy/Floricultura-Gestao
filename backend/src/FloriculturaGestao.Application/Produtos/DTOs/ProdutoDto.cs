namespace FloriculturaGestao.Application.Produtos.DTOs;

public record ProdutoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    string Sku,
    string Categoria,
    decimal PrecoCompra,
    decimal PrecoVenda,
    string? UnidadeMedida,
    bool Ativo,
    DateTime CriadoEm
);
