using FloriculturaGestao.Application.Contas.Commands;
using FloriculturaGestao.Application.Contas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContasController(IMediator mediator) : ControllerBase
{
    [HttpGet("pagar")]
    public async Task<IActionResult> ListarContasPagar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarContasPagarQuery(), ct);
        return Ok(resultado.Dados);
    }

    [HttpGet("receber")]
    public async Task<IActionResult> ListarContasReceber(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarContasReceberQuery(), ct);
        return Ok(resultado.Dados);
    }

    [HttpPost("pagar")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> CriarContaPagar([FromBody] CriarContaPagarCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }

    [HttpPost("receber")]
    [Authorize(Policy = "Vendedor")]
    public async Task<IActionResult> CriarContaReceber([FromBody] CriarContaReceberCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }

    [HttpPost("pagar/{id:guid}/pagar")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> PagarConta(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new PagarContaCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("receber/{id:guid}/receber")]
    [Authorize(Policy = "CaixaOperador")]
    public async Task<IActionResult> ReceberConta(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new ReceberContaCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }
}
