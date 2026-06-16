namespace PsicoManager.Domain.Enums;

/// <summary>
/// RNF03 — Trava de edição de prontuário. Após o prazo (padrão 24h) a
/// evolução fica 'Travada' e nenhum UPDATE/DELETE é permitido.
/// </summary>
public enum StatusEvolucao
{
    Editavel,
    Travada
}
