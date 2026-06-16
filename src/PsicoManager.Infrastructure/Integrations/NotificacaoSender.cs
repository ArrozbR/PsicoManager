using Microsoft.Extensions.Configuration;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Integrations;

/// <summary>
/// Despacha notificações pelos adaptadores de canal. WhatsAppAdapter (Business
/// Cloud API) e EmailAdapter (SMTP) são stubs que registram o envio; substituir
/// pelas integrações reais em produção. Canal 'ambos' usa os dois.
/// </summary>
public sealed class NotificacaoSender : INotificacaoSender
{
    private readonly bool _habilitado;

    public NotificacaoSender(IConfiguration config)
        => _habilitado = bool.TryParse(config["Notificacoes:Habilitado"], out var h) ? h : true;

    public async Task<bool> EnviarAsync(CanalNotificacao canal, string destino, string template,
        IReadOnlyDictionary<string, string> parametros, CancellationToken ct = default)
    {
        if (!_habilitado) return false;

        var ok = canal switch
        {
            CanalNotificacao.Whatsapp => await EnviarWhatsAppAsync(destino, template, ct),
            CanalNotificacao.Email => await EnviarEmailAsync(destino, template, ct),
            CanalNotificacao.Ambos =>
                await EnviarWhatsAppAsync(destino, template, ct)
                & await EnviarEmailAsync(destino, template, ct),
            _ => false
        };
        return ok;
    }

    private static Task<bool> EnviarWhatsAppAsync(string destino, string template, CancellationToken ct)
        => Task.FromResult(true); // WhatsApp Business Cloud API (stub)

    private static Task<bool> EnviarEmailAsync(string destino, string template, CancellationToken ct)
        => Task.FromResult(true); // SMTP (stub)
}
