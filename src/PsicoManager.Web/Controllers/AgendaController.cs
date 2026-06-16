using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsicoManager.Application.DTOs;
using PsicoManager.Application.UseCases.Agenda;

namespace PsicoManager.Web.Controllers;

/// <summary>Módulo Agenda — agendamento de sessões (UC01a).</summary>
[ApiController]
[Authorize(Roles = "psicologo")]
[Route("api/agenda")]
public sealed class AgendaController : ControllerBase
{
    private readonly IAgendarSessaoUseCase _agendar;
    public AgendaController(IAgendarSessaoUseCase agendar) => _agendar = agendar;

    /// <summary>UC01a — agenda uma sessão. 409 se houver conflito de horário (RN2).</summary>
    [HttpPost("sessoes")]
    [ProducesResponseType(typeof(SessaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Agendar([FromBody] AgendarSessaoRequest request, CancellationToken ct)
    {
        var resultado = await _agendar.ExecutarAsync(request, ct);
        return CreatedAtAction(nameof(Agendar), new { id = resultado.SessaoId }, resultado);
    }
}
