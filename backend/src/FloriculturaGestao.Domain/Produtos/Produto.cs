using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Produtos;

public enum CategoriaProduto
{
    Flor,
    Planta,
    Arranjo,
    Vaso,
    Insumo,
    Acessorio,
    Outro
}

public class Produto : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public CategoriaProduto Categoria { get; private set; }
    public decimal PrecoCompra { get; private set; }
    public decimal PrecoVenda { get; private set; }
    public string? UnidadeMedida { get; private set; }
    public bool Ativo { get; private set; } = true;

    private Produto() { }

    public static Produto Criar(string nome, string sku, CategoriaProduto categoria, decimal precoCompra, decimal precoVenda, string? descricao = null, string? unidadeMedida = "UN")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        if (precoVenda < 0) throw new ArgumentException("Preço de venda não pode ser negativo.");

        return new Produto
        {
            Nome = nome.Trim(),
            Sku = sku.Trim().ToUpperInvariant(),
            Categoria = categoria,
            PrecoCompra = precoCompra,
            PrecoVenda = precoVenda,
            Descricao = descricao?.Trim(),
            UnidadeMedida = unidadeMedida
        };
    }

    public void AtualizarPrecos(decimal precoCompra, decimal precoVenda)
    {
        if (precoVenda < 0) throw new ArgumentException("Preço de venda não pode ser negativo.");
        PrecoCompra = precoCompra;
        PrecoVenda = precoVenda;
        MarcarAtualizado();
    }

    public void Atualizar(string nome, string? descricao, CategoriaProduto categoria, decimal precoCompra, decimal precoVenda, string? unidadeMedida)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        Nome = nome.Trim();
        Descricao = descricao?.Trim();
        Categoria = categoria;
        PrecoCompra = precoCompra;
        PrecoVenda = precoVenda;
        UnidadeMedida = unidadeMedida;
        MarcarAtualizado();
    }

    public void Desativar() { Ativo = false; MarcarAtualizado(); }
    public void Ativar() { Ativo = true; MarcarAtualizado(); }
}
