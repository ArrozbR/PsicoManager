using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

/// <summary>RNF04 — apenas inserção e leitura; sem update/delete.</summary>
public sealed class LogAuditoriaRepository : ILogAuditoriaRepository
{
    private readonly PsicoManagerDbContext _db;
    public LogAuditoriaRepository(PsicoManagerDbContext db) => _db = db;

    public async Task AdicionarAsync(LogAuditoria log, CancellationToken ct = default)
        => await _db.LogsAuditoria.AddAsync(log, ct);

    public async Task<IReadOnlyList<LogAuditoria>> ListarPorEntidadeAsync(Guid entidadeId, CancellationToken ct = default)
        => await _db.LogsAuditoria.Where(l => l.EntidadeId == entidadeId)
            .OrderByDescending(l => l.DataHora).ToListAsync(ct);
}
