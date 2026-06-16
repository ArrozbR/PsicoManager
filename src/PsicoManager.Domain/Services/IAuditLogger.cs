using PsicoManager.Domain.Enums;

namespace PsicoManager.Domain.Services;

/// <summary>RNF04 — registro append-only de auditoria.</summary>
public interface IAuditLogger
{
    Task RegistrarAsync(Guid usuarioId, PerfilUsuario perfil, AcaoAuditoria acao,
        EntidadeAuditada entidade, Guid entidadeId, string ipOrigem,
        ResultadoAuditoria resultado, CancellationToken ct = default);
}
