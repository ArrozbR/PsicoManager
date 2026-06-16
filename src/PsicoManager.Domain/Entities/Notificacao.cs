using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: notificacao (EOBD). Máx. 3 tentativas (CHECK). RetryScheduler
/// reprograma envio em caso de falha (UC05).
/// </summary>
public class Notificacao : EntidadeBase
{
    public const short MaxTentativas = 3;

    public Guid SessaoId { get; private set; }
    public Guid DestinatarioId { get; private set; }
    public PerfilUsuario TipoDest { get; private set; }
    public string Tipo { get; private set; } = null!;       // lembrete_sessao|confirmacao|...
    public CanalNotificacao Canal { get; private set; }
    public StatusNotificacao Status { get; private set; } = StatusNotificacao.Pendente;
    public short Tentativas { get; private set; }
    public DateTime AgendadoPara { get; private set; }
    public DateTime? EnviadoEm { get; private set; }

    protected Notificacao() { } // EF

    public Notificacao(Guid sessaoId, Guid destinatarioId, PerfilUsuario tipoDest,
        string tipo, CanalNotificacao canal, DateTime agendadoParaUtc)
    {
        SessaoId = sessaoId;
        DestinatarioId = destinatarioId;
        TipoDest = tipoDest;
        Tipo = tipo;
        Canal = canal;
        AgendadoPara = agendadoParaUtc;
    }

    public void MarcarEnviada()
    {
        Status = StatusNotificacao.Enviado;
        EnviadoEm = DateTime.UtcNow;
    }

    /// <summary>Registra falha e reprograma se ainda houver tentativas.</summary>
    public bool RegistrarFalha(TimeSpan intervaloRetry)
    {
        Tentativas++;
        if (Tentativas >= MaxTentativas)
        {
            Status = StatusNotificacao.Falha;
            return false; // não reprograma
        }
        Status = StatusNotificacao.Pendente;
        AgendadoPara = DateTime.UtcNow.Add(intervaloRetry);
        return true; // reprogramada
    }
}
