using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class EvolucaoRepository : RepositorioBase<EvolucaoClinica>, IEvolucaoRepository
{
    public EvolucaoRepository(PsicoManagerDbContext db) : base(db) { }

    public override async Task<EvolucaoClinica?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await Db.Evolucoes.Include(e => e.Anexos).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<EvolucaoClinica>> ListarPorProntuarioAsync(Guid prontuarioId, CancellationToken ct = default)
        => await Db.Evolucoes.Include(e => e.Anexos)
            .Where(e => e.ProntuarioId == prontuarioId)
            .OrderByDescending(e => e.DataHora).ToListAsync(ct);

    public async Task<IReadOnlyList<EvolucaoClinica>> ListarVencidasNaoTravadasAsync(DateTime agoraUtc, CancellationToken ct = default)
        => await Db.Evolucoes
            .Where(e => e.Status == StatusEvolucao.Editavel && e.TravarEm <= agoraUtc)
            .ToListAsync(ct);
}
