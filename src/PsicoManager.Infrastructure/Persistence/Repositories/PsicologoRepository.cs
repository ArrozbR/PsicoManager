using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Infrastructure.Persistence.Repositories;

public sealed class PsicologoRepository : RepositorioBase<Psicologo>, IPsicologoRepository
{
    public PsicologoRepository(PsicoManagerDbContext db) : base(db) { }

    public Task<Psicologo?> ObterPorEmailAsync(string email, CancellationToken ct = default)
        => Db.Psicologos.FirstOrDefaultAsync(p => p.Email == email.ToLower(), ct);

    public Task<bool> EmailExisteAsync(string email, CancellationToken ct = default)
        => Db.Psicologos.AnyAsync(p => p.Email == email.ToLower(), ct);
}
