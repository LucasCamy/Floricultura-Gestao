using System.Security.Cryptography;

namespace FloriculturaGestao.Domain.Common;

public static class SenhaHasher
{
    private const int Iterations = 100_000;
    private const int KeyLength = 32;

    public static string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iterations, HashAlgorithmName.SHA256, KeyLength);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string senha, string senhaHash)
    {
        var partes = senhaHash.Split(':');
        if (partes.Length != 2) return false;

        var salt = Convert.FromBase64String(partes[0]);
        var hashEsperado = Convert.FromBase64String(partes[1]);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iterations, HashAlgorithmName.SHA256, KeyLength);

        return CryptographicOperations.FixedTimeEquals(hash, hashEsperado);
    }
}
