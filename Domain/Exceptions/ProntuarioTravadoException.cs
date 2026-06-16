namespace PsicoManager.Domain.Exceptions;

/// <summary>
/// RNF03 — tentativa de editar evolução após o prazo de trava. HTTP 403.
/// </summary>
public sealed class ProntuarioTravadoException : DomainException
{
    public ProntuarioTravadoException(string mensagem = "Prontuário travado — edição não permitida após o prazo.")
        : base(mensagem, 403) { }
}
