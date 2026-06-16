using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Financeiro;

/// <summary>UC04a — CO: lancarCobranca().</summary>
public interface ILancarCobrancaUseCase
{
    Task<CobrancaResponseDto> ExecutarAsync(LancarCobrancaRequest request, CancellationToken ct = default);
}
