using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Usuarios;

public class Usuario : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; } = PerfilUsuario.Vendedor;
    public bool Ativo { get; private set; } = true;

    private Usuario() { } // EF Core

    public static Usuario Criar(string nome, string email, string senha, PerfilUsuario perfil)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(senha);

        return new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = SenhaHasher.Hash(senha),
            Perfil = perfil
        };
    }

    public void Atualizar(string nome, string email, PerfilUsuario perfil)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Perfil = perfil;
        MarcarAtualizado();
    }

    public void RedefinirSenha(string novaSenha)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(novaSenha);
        SenhaHash = SenhaHasher.Hash(novaSenha);
        MarcarAtualizado();
    }

    public bool VerificarSenha(string senha) => SenhaHasher.Verificar(senha, SenhaHash);

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
}
