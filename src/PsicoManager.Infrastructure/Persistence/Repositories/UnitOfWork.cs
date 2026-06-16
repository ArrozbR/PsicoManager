using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly PsicoManagerDbContext _db;
    public UnitOfWork(PsicoManagerDbContext db) => _db = db;
    public Task<int> CommitAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
