using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class PacienteRepository : RepositorioBase<Paciente>, IPacienteRepository
{
    public PacienteRepository(PsicoManagerDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Paciente>> ListarPorPsicologoAsync(Guid psicologoId, CancellationToken ct = default)
        => await Db.Pacientes.Where(p => p.PsicologoId == psicologoId && p.Ativo)
            .OrderBy(p => p.Nome).ToListAsync(ct);
}
