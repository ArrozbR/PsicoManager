using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsicoManager.Application.DTOs;
using PsicoManager.Application.UseCases.Prontuario;

namespace PsicoManager.Web.Controllers;

/// <summary>Módulo Prontuário — operação CO-01 registrarEvolucao().</summary>
[ApiController]
[Authorize(Roles = "psicologo")]
[Route("api/prontuarios")]
public sealed class ProntuarioController : ControllerBase
{
    private readonly IRegistrarEvolucaoUseCase _registrarEvolucao;

    public ProntuarioController(IRegistrarEvolucaoUseCase registrarEvolucao)
        => _registrarEvolucao = registrarEvolucao;

    /// <summary>UC02a — registra uma evolução clínica (texto + anexos).</summary>
    [HttpPost("evolucoes")]
    [ProducesResponseType(typeof(EvolucaoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegistrarEvolucao(
        [FromBody] RegistrarEvolucaoRequest request, CancellationToken ct)
    {
        var resultado = await _registrarEvolucao.ExecutarAsync(request, ct);
        return CreatedAtAction(nameof(RegistrarEvolucao), new { id = resultado.EvolucaoId }, resultado);
    }
}
