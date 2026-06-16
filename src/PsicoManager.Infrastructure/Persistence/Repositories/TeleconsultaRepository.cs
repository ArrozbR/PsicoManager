using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class TeleconsultaRepository : RepositorioBase<Teleconsulta>, ITeleconsultaRepository
{
    public TeleconsultaRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<Teleconsulta?> ObterPorSessaoAsync(Guid sessaoId, CancellationToken ct = default)
        => Db.Teleconsultas.FirstOrDefaultAsync(t => t.SessaoId == sessaoId, ct);
}
