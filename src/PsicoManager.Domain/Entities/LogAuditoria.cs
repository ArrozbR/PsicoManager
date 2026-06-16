using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: log_auditoria (EOBD) — RNF04. Imutável: protegida por trigger
/// que impede UPDATE e DELETE. Toda CRUD sobre prontuário/cobrança gera entrada.
/// </summary>
public class LogAuditoria : EntidadeBase
{
    public Guid UsuarioId { get; private set; }
    public PerfilUsuario Perfil { get; private set; }
    public AcaoAuditoria Acao { get; private set; }
    public EntidadeAuditada Entidade { get; private set; }
    public Guid EntidadeId { get; private set; }
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public string IpOrigem { get; private set; } = null!;   // IPv4/IPv6 obrigatório
    public ResultadoAuditoria Resultado { get; private set; }

    protected LogAuditoria() { } // EF

    public LogAuditoria(Guid usuarioId, PerfilUsuario perfil, AcaoAuditoria acao,
        EntidadeAuditada entidade, Guid entidadeId, string ipOrigem,
        ResultadoAuditoria resultado)
    {
        UsuarioId = usuarioId;
        Perfil = perfil;
        Acao = acao;
        Entidade = entidade;
        EntidadeId = entidadeId;
        IpOrigem = string.IsNullOrWhiteSpace(ipOrigem) ? "desconhecido" : ipOrigem;
        Resultado = resultado;
    }
}
