using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: teleconsulta (EOBD). 1 sala por sessão (UK). Link único com UUID;
/// E2EE conforme Res. CFP 11/2018 (RNF02). Link expira ao encerrar a sessão.
/// </summary>
public class Teleconsulta : EntidadeBase
{
    public Guid SessaoId { get; private set; }
    public string LinkAcesso { get; private set; } = null!; // UK, URL única
    public StatusTeleconsulta Status { get; private set; } = StatusTeleconsulta.Aguardando;
    public short? DuracaoRealMin { get; private set; }
    public DateTime LinkExpiraEm { get; private set; }

    protected Teleconsulta() { } // EF

    public Teleconsulta(Guid sessaoId, string linkAcesso, DateTime linkExpiraEm)
    {
        SessaoId = sessaoId;
        LinkAcesso = linkAcesso;
        LinkExpiraEm = linkExpiraEm;
    }

    public void Iniciar() => Status = StatusTeleconsulta.EmAndamento;

    public void Encerrar(short duracaoRealMin)
    {
        Status = StatusTeleconsulta.Encerrada;
        DuracaoRealMin = duracaoRealMin;
        LinkExpiraEm = DateTime.UtcNow; // expira ao encerrar
    }
}
