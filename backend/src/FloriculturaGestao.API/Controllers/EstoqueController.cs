using FloriculturaGestao.Application.Estoque.Commands;
using FloriculturaGestao.Application.Estoque.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstoqueController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarEstoqueQuery(), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("baixo")]
    public async Task<IActionResult> ListarBaixo(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarEstoqueBaixoQuery(), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost("entrada")]
    [Authorize(Policy = "Estoquista")]
    public async Task<IActionResult> RegistrarEntrada([FromBody] RegistrarEntradaEstoqueCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }
}
