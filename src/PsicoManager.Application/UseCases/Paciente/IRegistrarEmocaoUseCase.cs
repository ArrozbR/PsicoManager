using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Paciente;

/// <summary>UC09 — Registrar Diário de Emoções.</summary>
public interface IRegistrarEmocaoUseCase
{
    Task<Guid> ExecutarAsync(RegistrarEmocaoRequest request, CancellationToken ct = default);
}
