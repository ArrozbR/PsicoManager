using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class VinculoRepository : RepositorioBase<VinculoTerapeutico>, IVinculoRepository
{
    public VinculoRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<bool> ExisteVinculoAtivoAsync(Guid psicologoId, Guid pacienteId, CancellationToken ct = default)
        => Db.Vinculos.AnyAsync(v => v.PsicologoId == psicologoId
                                     && v.PacienteId == pacienteId && v.Ativo, ct);
}
