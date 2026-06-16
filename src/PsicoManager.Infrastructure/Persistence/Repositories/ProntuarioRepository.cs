using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class ProntuarioRepository : RepositorioBase<ProntuarioClinico>, IProntuarioRepository
{
    public ProntuarioRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<ProntuarioClinico?> ObterPorPacienteAsync(Guid pacienteId, CancellationToken ct = default)
        => Db.Prontuarios.FirstOrDefaultAsync(p => p.PacienteId == pacienteId, ct);
}
