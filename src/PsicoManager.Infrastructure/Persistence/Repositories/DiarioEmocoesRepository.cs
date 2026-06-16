using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class DiarioEmocoesRepository : RepositorioBase<DiarioEmocoes>, IDiarioEmocoesRepository
{
    public DiarioEmocoesRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<DiarioEmocoes?> ObterPorPacienteAsync(Guid pacienteId, CancellationToken ct = default)
        => Db.Diarios.Include(d => d.Registros).FirstOrDefaultAsync(d => d.PacienteId == pacienteId, ct);
}
