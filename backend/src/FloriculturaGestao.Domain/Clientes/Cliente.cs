using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Clientes;

public class Cliente : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string? Cpf { get; private set; }
    public string? Endereco { get; private set; }
    public string? Observacoes { get; private set; }
    public bool Ativo { get; private set; } = true;

    private readonly List<Guid> _historicoVendas = [];
    public IReadOnlyCollection<Guid> HistoricoVendas => _historicoVendas.AsReadOnly();

    private Cliente() { } // EF Core

    public static Cliente Criar(string nome, string email, string telefone, string? cpf = null, string? endereco = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return new Cliente
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Telefone = telefone.Trim(),
            Cpf = cpf?.Trim(),
            Endereco = endereco?.Trim()
        };
    }

    public void Atualizar(string nome, string email, string telefone, string? cpf, string? endereco, string? observacoes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone.Trim();
        Cpf = cpf?.Trim();
        Endereco = endereco?.Trim();
        Observacoes = observacoes?.Trim();
        MarcarAtualizado();
    }

    public void Desativar()
    {
        Ativo = false;
        MarcarAtualizado();
    }

    public void Ativar()
    {
        Ativo = true;
        MarcarAtualizado();
    }

    public void RegistrarVenda(Guid vendaId)
    {
        _historicoVendas.Add(vendaId);
        MarcarAtualizado();
    }
}
