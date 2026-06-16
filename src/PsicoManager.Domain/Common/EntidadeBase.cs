namespace PsicoManager.Domain.Common;

/// <summary>
/// Base de toda entidade do domínio. UUID v4 como PK (convenção EOBD C4)
/// e timestamp de criação sempre em UTC.
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
}
