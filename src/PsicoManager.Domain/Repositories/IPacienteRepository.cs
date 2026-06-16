using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IPacienteRepository : IRepositorioBase<Paciente>
{
    Task<IReadOnlyList<Paciente>> ListarPorPsicologoAsync(Guid psicologoId, CancellationToken ct = default);
}
