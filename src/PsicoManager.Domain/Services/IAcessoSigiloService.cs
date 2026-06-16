using PsicoManager.Domain.Enums;

namespace PsicoManager.Domain.Services;

/// <summary>
/// RN1 / RNF01 — controle de sigilo do prontuário. Deve ser chamado ANTES
/// de qualquer acesso a dados clínicos (nota de design do CO-01).
/// </summary>
public interface IAcessoSigiloService
{
    /// <summary>
    /// Valida o vínculo terapêutico. Lança ConflitoAcessoException (HTTP 403)
    /// quando não houver vínculo ativo, registrando a tentativa no log de auditoria.
    /// </summary>
    Task ValidarAcessoAsync(Guid psicologoId, Guid pacienteId, string ipOrigem,
        EntidadeAuditada entidade, Guid entidadeId, CancellationToken ct = default);
}
