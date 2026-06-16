using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Common;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public abstract class RepositorioBase<T> : IRepositorioBase<T> where T : EntidadeBase
{
    protected readonly PsicoManagerDbContext Db;
    protected RepositorioBase(PsicoManagerDbContext db) => Db = db;

    public virtual async Task<T?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await Db.Set<T>().FindAsync(new object?[] { id }, ct);

    public virtual async Task AdicionarAsync(T entidade, CancellationToken ct = default)
        => await Db.Set<T>().AddAsync(entidade, ct);

    public virtual void Atualizar(T entidade) => Db.Set<T>().Update(entidade);
    public virtual void Remover(T entidade) => Db.Set<T>().Remove(entidade);
}
