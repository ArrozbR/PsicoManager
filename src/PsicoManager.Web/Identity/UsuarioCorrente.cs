using System.Security.Claims;
using PsicoManager.Application.Abstractions;
using PsicoManager.Domain.Enums;

namespace PsicoManager.Web.Identity;

/// <summary>
/// Implementa IUsuarioCorrente lendo as claims do JWT (camada Web). 'sub' →
/// UsuarioId, 'role' → Perfil, e o IP da conexão para o log de auditoria.
/// </summary>
public sealed class UsuarioCorrente : IUsuarioCorrente
{
    public Guid UsuarioId { get; }
    public PerfilUsuario Perfil { get; }
    public string IpOrigem { get; }

    public UsuarioCorrente(IHttpContextAccessor accessor)
    {
        var ctx = accessor.HttpContext;
        var user = ctx?.User;

        var sub = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user?.FindFirstValue("sub");
        UsuarioId = Guid.TryParse(sub, out var id) ? id : Guid.Empty;

        var role = user?.FindFirstValue(ClaimTypes.Role) ?? user?.FindFirstValue("role");
        Perfil = string.Equals(role, "paciente", StringComparison.OrdinalIgnoreCase)
            ? PerfilUsuario.Paciente : PerfilUsuario.Psicologo;

        IpOrigem = ctx?.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";
    }
}
