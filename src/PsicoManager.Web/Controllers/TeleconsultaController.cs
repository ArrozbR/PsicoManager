using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsicoManager.Application.DTOs;
using PsicoManager.Application.UseCases.Teleconsulta;

namespace PsicoManager.Web.Controllers;

/// <summary>Módulo Teleconsulta — inicia sala E2EE (UC03).</summary>
[ApiController]
[Authorize(Roles = "psicologo")]
[Route("api/teleconsultas")]
public sealed class TeleconsultaController : ControllerBase
{
    private readonly IIniciarTeleconsultaUseCase _iniciar;
    public TeleconsultaController(IIniciarTeleconsultaUseCase iniciar) => _iniciar = iniciar;

    /// <summary>UC03 — inicia (ou recupera) a sala virtual de uma sessão de teleconsulta.</summary>
    [HttpPost("{sessaoId:guid}/iniciar")]
    [ProducesResponseType(typeof(IniciarTeleconsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Iniciar(Guid sessaoId, CancellationToken ct)
        => Ok(await _iniciar.ExecutarAsync(sessaoId, ct));
}
