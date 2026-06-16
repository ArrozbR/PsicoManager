using PsicoManager.Domain.Enums;

namespace PsicoManager.Domain.Services;

/// <summary>Envio efetivo via adaptadores (WhatsApp Business API / SMTP).</summary>
public interface INotificacaoSender
{
    Task<bool> EnviarAsync(CanalNotificacao canal, string destino, string template,
        IReadOnlyDictionary<string, string> parametros, CancellationToken ct = default);
}
