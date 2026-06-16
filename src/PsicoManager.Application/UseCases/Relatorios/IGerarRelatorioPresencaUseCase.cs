using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Relatorios;

/// <summary>CSU09 — Gerar Relatório de Presença.</summary>
public interface IGerarRelatorioPresencaUseCase
{
    Task<RelatorioPresencaDto> ExecutarAsync(DateTime inicioUtc, DateTime fimUtc,
        Guid? pacienteId = null, CancellationToken ct = default);
}
