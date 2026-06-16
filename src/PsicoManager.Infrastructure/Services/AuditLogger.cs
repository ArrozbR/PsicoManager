using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Services;

/// <summary>RNF04 — grava entradas imutáveis no log de auditoria.</summary>
public sealed class AuditLogger : IAuditLogger
{
    private readonly ILogAuditoriaRepository _logs;
    public AuditLogger(ILogAuditoriaRepository logs) => _logs = logs;

    public Task RegistrarAsync(Guid usuarioId, PerfilUsuario perfil, AcaoAuditoria acao,
        EntidadeAuditada entidade, Guid entidadeId, string ipOrigem,
        ResultadoAuditoria resultado, CancellationToken ct = default)
    {
        var log = new LogAuditoria(usuarioId, perfil, acao, entidade, entidadeId, ipOrigem, resultado);
        return _logs.AdicionarAsync(log, ct);
    }
}
