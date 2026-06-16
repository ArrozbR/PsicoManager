using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsicoManager.Application.DTOs;
using PsicoManager.Application.UseCases.Financeiro;
using PsicoManager.Application.UseCases.Relatorios;

namespace PsicoManager.Web.Controllers;

/// <summary>Módulo Financeiro — cobranças (UC04a/b) e relatório mensal (CSU08).</summary>
[ApiController]
[Authorize(Roles = "psicologo")]
[Route("api/financeiro")]
public sealed class FinanceiroController : ControllerBase
{
    private readonly ILancarCobrancaUseCase _lancar;
    private readonly IConfirmarRecebimentoUseCase _confirmar;
    private readonly IGerarRelatorioFinanceiroUseCase _relatorio;

    public FinanceiroController(
        ILancarCobrancaUseCase lancar,
        IConfirmarRecebimentoUseCase confirmar,
        IGerarRelatorioFinanceiroUseCase relatorio)
    {
        _lancar = lancar;
        _confirmar = confirmar;
        _relatorio = relatorio;
    }

    /// <summary>UC04a — lança cobrança de uma sessão realizada (gera Pix se disponível).</summary>
    [HttpPost("cobrancas")]
    [ProducesResponseType(typeof(CobrancaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Lancar([FromBody] LancarCobrancaRequest request, CancellationToken ct)
    {
        var resultado = await _lancar.ExecutarAsync(request, ct);
        return CreatedAtAction(nameof(Lancar), new { id = resultado.CobrancaId }, resultado);
    }

    /// <summary>UC04b — confirma recebimento (baixa manual).</summary>
    [HttpPost("cobrancas/{id:guid}/confirmar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Confirmar(Guid id, CancellationToken ct)
    {
        await _confirmar.ExecutarAsync(id, ct);
        return NoContent();
    }

    /// <summary>CSU08 — relatório financeiro mensal.</summary>
    [HttpGet("relatorios/financeiro")]
    [ProducesResponseType(typeof(RelatorioFinanceiroDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RelatorioFinanceiro([FromQuery] int mes, [FromQuery] int ano, CancellationToken ct)
        => Ok(await _relatorio.ExecutarAsync(mes, ano, ct));
}
