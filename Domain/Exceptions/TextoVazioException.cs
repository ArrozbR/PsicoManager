namespace PsicoManager.Domain.Exceptions;

/// <summary>CO-01 — campo texto da evolução é nulo ou vazio. HTTP 422.</summary>
public sealed class TextoVazioException : DomainException
{
    public TextoVazioException(string mensagem = "O texto da evolução não pode ser vazio.")
        : base(mensagem, 422) { }
}
