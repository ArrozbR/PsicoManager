using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Prontuario;

/// <summary>UC02a — CO-01: registrarEvolucao().</summary>
public interface IRegistrarEvolucaoUseCase
{
    Task<EvolucaoResponseDto> ExecutarAsync(RegistrarEvolucaoRequest request, CancellationToken ct = default);
}
