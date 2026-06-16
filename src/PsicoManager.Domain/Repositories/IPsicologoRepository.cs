using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IPsicologoRepository : IRepositorioBase<Psicologo>
{
    Task<Psicologo?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExisteAsync(string email, CancellationToken ct = default);
}
