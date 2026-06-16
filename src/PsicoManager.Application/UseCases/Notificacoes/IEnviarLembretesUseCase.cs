namespace PsicoManager.Application.UseCases.Notificacoes;

/// <summary>UC05 — Enviar Lembretes Automáticos (background, com retry).</summary>
public interface IEnviarLembretesUseCase
{
    Task<int> ProcessarPendentesAsync(CancellationToken ct = default);
}
