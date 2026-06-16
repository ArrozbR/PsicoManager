namespace PsicoManager.Domain.Exceptions;

/// <summary>
/// RN2 — já existe sessão no mesmo horário (extensão 4a do UC01a).
/// HTTP 409 Conflict.
/// </summary>
public sealed class ConflitoHorarioException : DomainException
{
    public ConflitoHorarioException(string mensagem = "Horário já ocupado — escolha outro horário.")
        : base(mensagem, 409) { }
}
