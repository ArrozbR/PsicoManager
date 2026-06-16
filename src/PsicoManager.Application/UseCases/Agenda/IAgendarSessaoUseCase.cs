using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Agenda;

/// <summary>UC01a — Agendar Sessão (psicólogo).</summary>
public interface IAgendarSessaoUseCase
{
    Task<SessaoResponseDto> ExecutarAsync(AgendarSessaoRequest request, CancellationToken ct = default);
}
