using PsicoManager.Domain.Common;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Vínculo terapêutico ativo entre psicólogo e paciente. Base do controle
/// de sigilo (RN1 / RNF01): só há acesso ao prontuário se o vínculo existir.
/// </summary>
public class VinculoTerapeutico : EntidadeBase
{
    public Guid PsicologoId { get; private set; }
    public Guid PacienteId { get; private set; }
    public bool Ativo { get; private set; } = true;
    public DateTime? EncerradoEm { get; private set; }

    protected VinculoTerapeutico() { } // EF

    public VinculoTerapeutico(Guid psicologoId, Guid pacienteId)
    {
        PsicologoId = psicologoId;
        PacienteId = pacienteId;
    }

    public void Encerrar()
    {
        Ativo = false;
        EncerradoEm = DateTime.UtcNow;
    }
}
