using FloriculturaGestao.Application.Common;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Contas;
using MediatR;

namespace FloriculturaGestao.Application.Contas.Commands;

public record CriarContaPagarCommand(
    Guid FornecedorId,
    string Descricao,
    decimal Valor,
    DateTime DataVencimento,
    string? Observacoes
) : IRequest<ResultadoOperacao<Guid>>;

public class CriarContaPagarHandler(IContaPagarRepositorio repo, IUnitOfWork uow)
    : IRequestHandler<CriarContaPagarCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(CriarContaPagarCommand request, CancellationToken ct)
    {
        var conta = ContaPagar.Criar(request.FornecedorId, request.Descricao, request.Valor, request.DataVencimento, request.Observacoes);
        await repo.AdicionarAsync(conta, ct);
        await uow.SaveChangesAsync(ct);
        return ResultadoOperacao<Guid>.Ok(conta.Id, "Conta a pagar criada.");
    }
}

public record CriarContaReceberCommand(
    Guid ClienteId,
    string Descricao,
    decimal Valor,
    DateTime DataVencimento,
    Guid? VendaId,
    string? Observacoes
) : IRequest<ResultadoOperacao<Guid>>;

public class CriarContaReceberHandler(IContaReceberRepositorio repo, IUnitOfWork uow)
    : IRequestHandler<CriarContaReceberCommand, ResultadoOperacao<Guid>>
{
    public async Task<ResultadoOperacao<Guid>> Handle(CriarContaReceberCommand request, CancellationToken ct)
    {
        var conta = ContaReceber.Criar(request.ClienteId, request.Descricao, request.Valor, request.DataVencimento, request.VendaId, request.Observacoes);
        await repo.AdicionarAsync(conta, ct);
        await uow.SaveChangesAsync(ct);
        return ResultadoOperacao<Guid>.Ok(conta.Id, "Conta a receber criada.");
    }
}

public record PagarContaCommand(Guid ContaId) : IRequest<ResultadoOperacao>;

public class PagarContaHandler(IContaPagarRepositorio repo, IUnitOfWork uow)
    : IRequestHandler<PagarContaCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(PagarContaCommand request, CancellationToken ct)
    {
        var conta = await repo.ObterPorIdAsync(request.ContaId, ct);
        if (conta is null) return ResultadoOperacao.Falha("Conta não encontrada.");
        conta.RegistrarPagamento();
        repo.Atualizar(conta);
        await uow.SaveChangesAsync(ct);
        return ResultadoOperacao.Ok("Pagamento registrado.");
    }
}

public record ReceberContaCommand(Guid ContaId) : IRequest<ResultadoOperacao>;

public class ReceberContaHandler(IContaReceberRepositorio repo, IUnitOfWork uow)
    : IRequestHandler<ReceberContaCommand, ResultadoOperacao>
{
    public async Task<ResultadoOperacao> Handle(ReceberContaCommand request, CancellationToken ct)
    {
        var conta = await repo.ObterPorIdAsync(request.ContaId, ct);
        if (conta is null) return ResultadoOperacao.Falha("Conta não encontrada.");
        conta.RegistrarRecebimento();
        repo.Atualizar(conta);
        await uow.SaveChangesAsync(ct);
        return ResultadoOperacao.Ok("Recebimento registrado.");
    }
}
