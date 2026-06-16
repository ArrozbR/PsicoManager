namespace PsicoManager.Domain.Enums;

/// <summary>
/// Status da sessão. Só avança no sentido natural (RN2): o status é
/// calculado a partir dos registros de movimentação da agenda e não é
/// editável retroativamente.
/// </summary>
public enum StatusSessao
{
    Agendada,
    Realizada,
    Falta,       // no-show
    Cancelada
}
