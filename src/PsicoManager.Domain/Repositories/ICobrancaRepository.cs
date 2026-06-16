using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface ICobrancaRepository : IRepositorioBase<Cobranca>
{
    Task<Cobranca?> ObterPorSessaoAsync(Guid sessaoId, CancellationToken ct = default);
    Task<IReadOnlyList<Cobranca>> ListarPorPeriodoAsync(
        Guid psicologoId, DateOnly inicio, DateOnly fim, CancellationToken ct = default);
}
