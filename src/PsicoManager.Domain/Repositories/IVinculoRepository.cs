using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IVinculoRepository : IRepositorioBase<VinculoTerapeutico>
{
    /// <summary>RN1 — existe vínculo ATIVO entre o psicólogo e o paciente?</summary>
    Task<bool> ExisteVinculoAtivoAsync(Guid psicologoId, Guid pacienteId, CancellationToken ct = default);
}
