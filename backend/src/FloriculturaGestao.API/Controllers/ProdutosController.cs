using FloriculturaGestao.Application.Produtos.Commands;
using FloriculturaGestao.Application.Produtos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FloriculturaGestao.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ListarProdutosQuery(), ct);
        return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
    }

    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoCommand command, CancellationToken ct)
    {
        var resultado = await mediator.Send(command, ct);
        return resultado.Sucesso ? Created("", resultado) : BadRequest(resultado);
    }
}
