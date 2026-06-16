using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class SessaoRepository : RepositorioBase<Sessao>, ISessaoRepository
{
    public SessaoRepository(PsicoManagerDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Sessao>> BuscarConflitosAsync(
        Guid psicologoId, DateTime inicioUtc, int duracaoMin, CancellationToken ct = default)
    {
        var fim = inicioUtc.AddMinutes(duracaoMin);
        // Sobreposição de intervalos: existe(início < fimNovo && fimExistente > inícioNovo)
        return await Db.Sessoes
            .Where(s => s.PsicologoId == psicologoId
                        && s.Status != StatusSessao.Cancelada
                        && s.DataHora < fim
                        && s.DataHora.AddMinutes(s.DuracaoMin) > inicioUtc)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Sessao>> ListarPorPeriodoAsync(
        Guid psicologoId, DateTime inicioUtc, DateTime fimUtc, CancellationToken ct = default)
        => await Db.Sessoes
            .Where(s => s.PsicologoId == psicologoId && s.DataHora >= inicioUtc && s.DataHora <= fimUtc)
            .OrderBy(s => s.DataHora).ToListAsync(ct);

    public Task<bool> ExisteSessaoRealizadaAsync(Guid sessaoId, CancellationToken ct = default)
        => Db.Sessoes.AnyAsync(s => s.Id == sessaoId && s.Status == StatusSessao.Realizada, ct);
}
