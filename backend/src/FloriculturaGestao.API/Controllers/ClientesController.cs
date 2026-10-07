using FloriculturaGestao.Application.Clientes.Commands;
using FloriculturaGestao.Application.Clientes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarClientesQuery(), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new ObterClientePorIdQuery(id), ct);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }

    [HttpPost]
    [Authorize(Policy = "Vendedor")]
    public async Task<IActionResult> Criar([FromBody] CriarClienteCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso
            ? CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados }, resultado)
            : BadRequest(resultado);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Vendedor")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarClienteCommand command, CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest(new { mensagem = "ID da rota não corresponde ao ID do corpo." });

        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Vendedor")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new DesativarClienteCommand(id), ct);
        return resultado.Sucesso ? Ok(resultado) : NotFound(resultado);
    }
}
