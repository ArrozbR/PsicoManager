namespace PsicoManager.Domain.Exceptions;

/// <summary>Exceção base de regra de negócio. Mapeada para HTTP 4xx na Web.</summary>
public abstract class DomainException : Exception
{
    public int HttpStatus { get; }
    protected DomainException(string mensagem, int httpStatus) : base(mensagem)
        => HttpStatus = httpStatus;
}
