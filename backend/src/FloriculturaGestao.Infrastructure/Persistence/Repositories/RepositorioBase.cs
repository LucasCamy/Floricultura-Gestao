using FloriculturaGestao.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace FloriculturaGestao.Infrastructure.Persistence.Repositories;

public abstract class RepositorioBase<T>(FloriculturaDbContext context) : IRepositorio<T>
    where T : EntidadeBase
{
    protected readonly FloriculturaDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.FindAsync([id], ct);

    public async Task<IReadOnlyList<T>> ObterTodosAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking().ToListAsync(ct);

    public async Task AdicionarAsync(T entidade, CancellationToken ct = default)
        => await DbSet.AddAsync(entidade, ct);

    public void Atualizar(T entidade) => DbSet.Update(entidade);

    public void Remover(T entidade) => DbSet.Remove(entidade);
}
