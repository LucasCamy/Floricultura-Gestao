using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Fornecedores;

public class Fornecedor : EntidadeBase
{
    public string RazaoSocial { get; private set; } = string.Empty;
    public string NomeFantasia { get; private set; } = string.Empty;
    public string Cnpj { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Endereco { get; private set; }
    public bool Ativo { get; private set; } = true;

    private Fornecedor() { }

    public static Fornecedor Criar(string razaoSocial, string nomeFantasia, string cnpj, string telefone, string? email = null, string? endereco = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(razaoSocial);
        ArgumentException.ThrowIfNullOrWhiteSpace(cnpj);

        return new Fornecedor
        {
            RazaoSocial = razaoSocial.Trim(),
            NomeFantasia = nomeFantasia.Trim(),
            Cnpj = cnpj.Trim(),
            Telefone = telefone.Trim(),
            Email = email?.Trim(),
            Endereco = endereco?.Trim()
        };
    }

    public void Atualizar(string razaoSocial, string nomeFantasia, string telefone, string? email, string? endereco)
    {
        RazaoSocial = razaoSocial.Trim();
        NomeFantasia = nomeFantasia.Trim();
        Telefone = telefone.Trim();
        Email = email?.Trim();
        Endereco = endereco?.Trim();
        MarcarAtualizado();
    }

    public void Desativar() { Ativo = false; MarcarAtualizado(); }
    public void Ativar() { Ativo = true; MarcarAtualizado(); }
}
