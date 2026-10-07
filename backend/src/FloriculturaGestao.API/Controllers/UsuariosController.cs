using FloriculturaGestao.Application.Usuarios.Commands;
using FloriculturaGestao.Application.Usuarios.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class UsuariosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarUsuariosQuery(), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarUsuarioCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(new { mensagem = "ID da rota não corresponde ao ID do corpo." });

        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("{id:guid}/redefinir-senha")]
    public async Task<IActionResult> RedefinirSenha(Guid id, [FromBody] RedefinirSenhaRequest request, CancellationToken ct)
    {
        var resultado = await mediator.Send(new RedefinirSenhaCommand(id, request.NovaSenha), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new DesativarUsuarioCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new AtivarUsuarioCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    public record RedefinirSenhaRequest(string NovaSenha);
}
