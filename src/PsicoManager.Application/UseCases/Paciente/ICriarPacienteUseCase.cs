using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Paciente;

public interface ICriarPacienteUseCase
{
    Task<PacienteDto> ExecutarAsync(CriarPacienteRequest request, CancellationToken ct = default);
}
