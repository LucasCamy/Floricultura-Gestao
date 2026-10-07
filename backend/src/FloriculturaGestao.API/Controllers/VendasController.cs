using FloriculturaGestao.Application.Vendas.Commands;
using FloriculturaGestao.Application.Vendas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendasController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "Vendedor")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarVendaCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> ListarPorPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim, CancellationToken ct)
    {
        // Querystring dates arrive as Unspecified; normalize to UTC for PostgreSQL timestamptz.
        var inicioUtc = DateTime.SpecifyKind(inicio.Date, DateTimeKind.Utc);
        var fimUtc = DateTime.SpecifyKind(fim.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

        var resultado = await mediator.Send(new ListarVendasPorPeriodoQuery(inicioUtc, fimUtc), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }
}
