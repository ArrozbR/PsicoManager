using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class CobrancaRepository : RepositorioBase<Cobranca>, ICobrancaRepository
{
    public CobrancaRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<Cobranca?> ObterPorSessaoAsync(Guid sessaoId, CancellationToken ct = default)
        => Db.Cobrancas.FirstOrDefaultAsync(c => c.SessaoId == sessaoId, ct);

    public async Task<IReadOnlyList<Cobranca>> ListarPorPeriodoAsync(
        Guid psicologoId, DateOnly inicio, DateOnly fim, CancellationToken ct = default)
        => await Db.Cobrancas
            .Where(c => c.PsicologoId == psicologoId
                        && c.DataLancamento >= inicio && c.DataLancamento <= fim)
            .ToListAsync(ct);
}
