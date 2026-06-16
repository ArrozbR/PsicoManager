using PsicoManager.Domain.Enums;

namespace PsicoManager.Application.Abstractions;

/// <summary>Contexto do usuário autenticado (extraído do JWT na camada Web).</summary>
public interface IUsuarioCorrente
{
    Guid UsuarioId { get; }
    PerfilUsuario Perfil { get; }
    string IpOrigem { get; }
}
