namespace PsicoManager.Domain.Exceptions;

/// <summary>CO-01 — tipo de arquivo não suportado ou tamanho > 10 MB. HTTP 422.</summary>
public sealed class ArquivoInvalidoException : DomainException
{
    public ArquivoInvalidoException(string mensagem)
        : base(mensagem, 422) { }
}
