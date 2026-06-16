using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IDiarioEmocoesRepository : IRepositorioBase<DiarioEmocoes>
{
    Task<DiarioEmocoes?> ObterPorPacienteAsync(Guid pacienteId, CancellationToken ct = default);
}
