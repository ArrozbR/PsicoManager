using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IProntuarioRepository : IRepositorioBase<ProntuarioClinico>
{
    Task<ProntuarioClinico?> ObterPorPacienteAsync(Guid pacienteId, CancellationToken ct = default);
}
