using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

/// <summary>RNF04 — repositório append-only (sem Atualizar/Remover).</summary>
public interface ILogAuditoriaRepository
{
    Task AdicionarAsync(LogAuditoria log, CancellationToken ct = default);
    Task<IReadOnlyList<LogAuditoria>> ListarPorEntidadeAsync(Guid entidadeId, CancellationToken ct = default);
}
