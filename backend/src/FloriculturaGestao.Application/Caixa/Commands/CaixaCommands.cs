using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Caixa;
using FloriculturaGestao.Domain.Common;
using MediatR;

namespace FloriculturaGestao.Application.Caixa.Commands;

public record AbrirCaixaCommand(decimal SaldoInicial, string OperadorId) : IRequest<ResultadoOperacao<Guid>>;

public class AbrirCaixaHandler(ICaixaRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<AbrirCaixaCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(AbrirCaixaCommand request, CancellationToken ct)
    {
        var aberto = await repositorio.ObterCaixaAbertoAsync(ct);
        if (aberto is not null)
            return ResultadoOperacao<Guid>.Falha("Já existe um caixa aberto. Feche-o antes de abrir outro.");

        var caixa = Domain.Caixa.Caixa.Abrir(request.SaldoInicial, request.OperadorId);
        await repositorio.AdicionarAsync(caixa, ct);
        await uow.SaveChangesAsync(ct);

        return ResultadoOperacao<Guid>.Ok(caixa.Id, "Caixa aberto com sucesso.");
    }
}

public record FecharCaixaCommand(Guid CaixaId) : IRequest<ResultadoOperacao>;

public class FecharCaixaHandler(ICaixaRepositorio repositorio, IUnitOfWork uow)
    : IRequestHandler<FecharCaixaCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(FecharCaixaCommand request, CancellationToken ct)
    {
        var caixa = await repositorio.ObterPorIdAsync(request.CaixaId, ct);
        if (caixa is null) return ResultadoOperacao.Falha("Caixa não encontrado.");

        caixa.Fechar();
        repositorio.Atualizar(caixa);
        await uow.SaveChangesAsync(ct);

        return ResultadoOperacao.Ok("Caixa fechado com sucesso.");
    }
}
