using PsicoManager.Domain.Common;

namespace PsicoManager.Domain.Repositories;

public interface IRepositorioBase<T> where T : EntidadeBase
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task AdicionarAsync(T entidade, CancellationToken ct = default);
    void Atualizar(T entidade);
    void Remover(T entidade);
}
