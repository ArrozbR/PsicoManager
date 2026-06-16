using PsicoManager.Domain.Common;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: diario_emocoes (EOBD). 1 diário por paciente (UK). O paciente
/// decide se compartilha o diário com o psicólogo.
/// </summary>
public class DiarioEmocoes : EntidadeBase
{
    public Guid PacienteId { get; private set; }
    public bool Compartilhado { get; private set; }

    private readonly List<RegistroEmocao> _registros = new();
    public IReadOnlyCollection<RegistroEmocao> Registros => _registros.AsReadOnly();

    protected DiarioEmocoes() { } // EF

    public DiarioEmocoes(Guid pacienteId) => PacienteId = pacienteId;

    public void AlternarCompartilhamento(bool compartilhar) => Compartilhado = compartilhar;
    public void AdicionarRegistro(RegistroEmocao r) => _registros.Add(r);
}
