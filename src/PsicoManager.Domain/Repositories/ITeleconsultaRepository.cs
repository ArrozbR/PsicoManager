using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface ITeleconsultaRepository : IRepositorioBase<Teleconsulta>
{
    Task<Teleconsulta?> ObterPorSessaoAsync(Guid sessaoId, CancellationToken ct = default);
}
