namespace PsicoManager.Domain.Exceptions;

/// <summary>
/// RNF05 / RN3 — tentativa de excluir paciente com histórico inferior a 5 anos.
/// </summary>
public sealed class RetencaoObrigatoriaException : DomainException
{
    public RetencaoObrigatoriaException(string mensagem = "Retenção obrigatória não cumprida (mínimo 5 anos).")
        : base(mensagem, 409) { }
}
