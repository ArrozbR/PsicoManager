using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class NotificacaoRepository : RepositorioBase<Notificacao>, INotificacaoRepository
{
    public NotificacaoRepository(PsicoManagerDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Notificacao>> ListarPendentesAsync(DateTime ateUtc, CancellationToken ct = default)
        => await Db.Notificacoes
            .Where(n => n.Status == StatusNotificacao.Pendente && n.AgendadoPara <= ateUtc)
            .ToListAsync(ct);
}
