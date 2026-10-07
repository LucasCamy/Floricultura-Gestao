using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FloriculturaGestao.Domain.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IConfiguration configuration,
    IUsuarioRepositorio usuarioRepositorio
) : ControllerBase
{
    public record LoginRequest(string Email, string Senha);
    public record LoginResponse(string Token, string Nome, string Email, string Role, DateTime Expiracao);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await usuarioRepositorio.ObterPorEmailAsync(request.Email.Trim().ToLowerInvariant());

        if (usuario is null || !usuario.Ativo || !usuario.VerificarSenha(request.Senha))
            return Unauthorized(new { mensagem = "Credenciais inválidas." });

        var jwtSettings = configuration.GetSection("Jwt");
        var key = jwtSettings["Key"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expiresMinutes = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "480");

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiracao = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiracao,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(tokenString, usuario.Nome, usuario.Email, usuario.Perfil.ToString(), expiracao));
    }
}
