using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface INotificacaoRepository : IRepositorioBase<Notificacao>
{
    Task<IReadOnlyList<Notificacao>> ListarPendentesAsync(DateTime ateUtc, CancellationToken ct = default);
}
