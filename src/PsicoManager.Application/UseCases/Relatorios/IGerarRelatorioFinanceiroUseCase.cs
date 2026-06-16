using PsicoManager.Application.DTOs;

namespace PsicoManager.Application.UseCases.Relatorios;

/// <summary>CSU08 — Gerar Relatório Financeiro Mensal.</summary>
public interface IGerarRelatorioFinanceiroUseCase
{
    Task<RelatorioFinanceiroDto> ExecutarAsync(int mes, int ano, CancellationToken ct = default);
}
