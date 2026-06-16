namespace PsicoManager.Domain.Exceptions;

/// <summary>Violação genérica de regra de negócio. HTTP 422.</summary>
public sealed class RegraDeNegocioException : DomainException
{
    public RegraDeNegocioException(string mensagem) : base(mensagem, 422) { }
}
