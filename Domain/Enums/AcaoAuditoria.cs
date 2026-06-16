namespace PsicoManager.Domain.Enums;

/// <summary>RNF04 — ações registradas no log de auditoria imutável.</summary>
public enum AcaoAuditoria
{
    Criacao,
    Leitura,
    Edicao,
    Exclusao,
    AcessoNegado
}
