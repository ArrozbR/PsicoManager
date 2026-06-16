namespace PsicoManager.Domain.Repositories;

/// <summary>Confirma a transação (uma persistência por caso de uso).</summary>
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
