using System.Security.Claims;
using FloriculturaGestao.Domain.Auditoria;
using FloriculturaGestao.Domain.Caixa;
using FloriculturaGestao.Domain.Clientes;
using FloriculturaGestao.Domain.Common;
using FloriculturaGestao.Domain.Contas;
using FloriculturaGestao.Domain.Estoque;
using FloriculturaGestao.Domain.Fornecedores;
using FloriculturaGestao.Domain.Produtos;
using FloriculturaGestao.Domain.Usuarios;
using FloriculturaGestao.Domain.Vendas;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence;

public class FloriculturaDbContext(
    DbContextOptions<FloriculturaDbContext> options,
    IHttpContextAccessor? httpContextAccessor = null
) : DbContext(options), IUnitOfWork
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<EstoqueItem> EstoqueItens => Set<EstoqueItem>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Caixa> Caixas => Set<Caixa>();
    public DbSet<MovimentacaoCaixa> MovimentacoesCaixa => Set<MovimentacaoCaixa>();
    public DbSet<ContaPagar> ContasPagar => Set<ContaPagar>();
    public DbSet<ContaReceber> ContasReceber => Set<ContaReceber>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FloriculturaDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var realizadoPor = httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? "sistema";

        var auditorias = ChangeTracker.Entries<EntidadeBase>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => new RegistroAuditoria
            {
                Tabela = e.Metadata.GetTableName() ?? e.Entity.GetType().Name,
                RegistroId = e.Entity.Id.ToString(),
                Acao = e.State switch
                {
                    EntityState.Added => "Criou",
                    EntityState.Modified => "Atualizou",
                    EntityState.Deleted => "Excluiu",
                    _ => "?"
                },
                RealizadoPor = realizadoPor,
                RealizadoEm = DateTime.UtcNow
            })
            .ToList();

        var resultado = await base.SaveChangesAsync(cancellationToken);

        if (auditorias.Count > 0)
        {
            await RegistrosAuditoria.AddRangeAsync(auditorias, cancellationToken);
            await base.SaveChangesAsync(cancellationToken);
        }

        return resultado;
    }
}
