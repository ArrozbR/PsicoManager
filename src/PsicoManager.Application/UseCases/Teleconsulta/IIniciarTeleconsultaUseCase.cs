using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Teleconsulta;

/// <summary>UC03 — Realizar Teleconsulta (psicólogo).</summary>
public interface IIniciarTeleconsultaUseCase
{
    Task<IniciarTeleconsultaResponse> ExecutarAsync(Guid sessaoId, CancellationToken ct = default);
}
