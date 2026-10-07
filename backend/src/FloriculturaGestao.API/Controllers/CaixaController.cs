using FloriculturaGestao.Application.Caixa.Commands;
using FloriculturaGestao.Application.Caixa.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CaixaOperador")]
public class CaixaController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarCaixasQuery(), ct);
        return Ok(resultado.Dados);
    }

    [HttpPost("abrir")]
    public async Task<IActionResult> Abrir([FromBody] AbrirCaixaCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }

    [HttpPost("{id:guid}/fechar")]
    public async Task<IActionResult> Fechar(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new FecharCaixaCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }
}
