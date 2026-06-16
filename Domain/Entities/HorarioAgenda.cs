using PsicoManager.Domain.Common;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>Tabela: horario_agenda (EOBD). Slots de disponibilidade e bloqueios.</summary>
public class HorarioAgenda : EntidadeBase
{
    public Guid PsicologoId { get; private set; }
    public DateOnly Data { get; private set; }
    public TimeOnly HoraInicio { get; private set; }       // múltipla de 15 min
    public short DuracaoMin { get; private set; } = 50;     // 30–240
    public string Tipo { get; private set; } = "disponivel"; // disponivel|ocupado|bloqueado
    public string Recorrencia { get; private set; } = "unica"; // unica|semanal|quinzenal|mensal
    public DateOnly? DataTermino { get; private set; }       // obrigatória se recorrencia≠unica
    public Guid? SessaoId { get; private set; }              // FK quando tipo=ocupado

    protected HorarioAgenda() { } // EF

    public HorarioAgenda(Guid psicologoId, DateOnly data, TimeOnly horaInicio,
        short duracaoMin = 50, string tipo = "disponivel",
        string recorrencia = "unica", DateOnly? dataTermino = null)
    {
        if (horaInicio.Minute % 15 != 0)
            throw new RegraDeNegocioException("Hora de início deve ser múltipla de 15 minutos.");
        if (duracaoMin < 30 || duracaoMin > 240)
            throw new RegraDeNegocioException("Duração deve estar entre 30 e 240 minutos.");
        if (recorrencia != "unica" && dataTermino is null)
            throw new RegraDeNegocioException("Data de término obrigatória para recorrência.");

        PsicologoId = psicologoId;
        Data = data;
        HoraInicio = horaInicio;
        DuracaoMin = duracaoMin;
        Tipo = tipo;
        Recorrencia = recorrencia;
        DataTermino = dataTermino;
    }

    public void Ocupar(Guid sessaoId) { Tipo = "ocupado"; SessaoId = sessaoId; }
    public void Bloquear() => Tipo = "bloqueado";
    public void Liberar() { Tipo = "disponivel"; SessaoId = null; }
}
