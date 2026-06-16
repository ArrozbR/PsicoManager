using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: sessao (EOBD). Status só avança (RN2) e a movimentação é
/// registrada para cálculo do relatório de presença (CSU09).
/// </summary>
public class Sessao : EntidadeBase
{
    public Guid PsicologoId { get; private set; }
    public Guid PacienteId { get; private set; }
    public DateTime DataHora { get; private set; }         // UTC — não pode ser passada no agendamento
    public short DuracaoMin { get; private set; } = 50;    // 30–240
    public TipoSessao Tipo { get; private set; } = TipoSessao.Presencial;
    public StatusSessao Status { get; private set; } = StatusSessao.Agendada;
    public string? MotivoCancelamento { get; private set; }
    public DateTime AtualizadoEm { get; private set; } = DateTime.UtcNow;

    protected Sessao() { } // EF

    public Sessao(Guid psicologoId, Guid pacienteId, DateTime dataHoraUtc,
        TipoSessao tipo, short duracaoMin = 50)
    {
        if (dataHoraUtc <= DateTime.UtcNow)
            throw new RegraDeNegocioException("A data da sessão não pode estar no passado.");
        if (duracaoMin < 30 || duracaoMin > 240)
            throw new RegraDeNegocioException("Duração deve estar entre 30 e 240 minutos.");

        PsicologoId = psicologoId;
        PacienteId = pacienteId;
        DataHora = dataHoraUtc;
        Tipo = tipo;
        DuracaoMin = duracaoMin;
    }

    public void MarcarRealizada()
    {
        if (Status != StatusSessao.Agendada)
            throw new RegraDeNegocioException("Só sessões agendadas podem ser realizadas (status só avança).");
        Status = StatusSessao.Realizada;
        Touch();
    }

    public void MarcarFalta()
    {
        if (Status != StatusSessao.Agendada)
            throw new RegraDeNegocioException("Só sessões agendadas podem ser marcadas como falta.");
        Status = StatusSessao.Falta;
        Touch();
    }

    public void Cancelar(string motivo)
    {
        if (Status is StatusSessao.Realizada or StatusSessao.Cancelada)
            throw new RegraDeNegocioException("Sessão não pode mais ser cancelada.");
        if (string.IsNullOrWhiteSpace(motivo))
            throw new RegraDeNegocioException("Motivo de cancelamento é obrigatório.");
        Status = StatusSessao.Cancelada;
        MotivoCancelamento = motivo.Trim();
        Touch();
    }

    public void Reagendar(DateTime novaDataHoraUtc)
    {
        if (Status != StatusSessao.Agendada)
            throw new RegraDeNegocioException("Só sessões agendadas podem ser reagendadas.");
        if (novaDataHoraUtc <= DateTime.UtcNow)
            throw new RegraDeNegocioException("A nova data não pode estar no passado.");
        DataHora = novaDataHoraUtc;
        Touch();
    }

    private void Touch() => AtualizadoEm = DateTime.UtcNow;
}
