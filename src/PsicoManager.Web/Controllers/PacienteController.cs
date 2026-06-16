using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsicoManager.Application.DTOs;
using PsicoManager.Application.UseCases.Paciente;
using PsicoManager.Application.UseCases.Relatorios;

namespace PsicoManager.Web.Controllers;

/// <summary>Módulo Paciente — cadastro, relatório de presença (CSU09) e diário (UC09).</summary>
[ApiController]
[Authorize]
[Route("api/pacientes")]
public sealed class PacienteController : ControllerBase
{
    private readonly ICriarPacienteUseCase _criar;
    private readonly IRegistrarEmocaoUseCase _registrarEmocao;
    private readonly IGerarRelatorioPresencaUseCase _presenca;

    public PacienteController(
        ICriarPacienteUseCase criar,
        IRegistrarEmocaoUseCase registrarEmocao,
        IGerarRelatorioPresencaUseCase presenca)
    {
        _criar = criar;
        _registrarEmocao = registrarEmocao;
        _presenca = presenca;
    }

    /// <summary>Cadastra paciente + prontuário + vínculo (psicólogo).</summary>
    [HttpPost]
    [Authorize(Roles = "psicologo")]
    [ProducesResponseType(typeof(PacienteDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarPacienteRequest request, CancellationToken ct)
    {
        var dto = await _criar.ExecutarAsync(request, ct);
        return CreatedAtAction(nameof(Criar), new { id = dto.Id }, dto);
    }

    /// <summary>UC09 — registra entrada no diário de emoções (paciente).</summary>
    [HttpPost("diario/emocoes")]
    [Authorize(Roles = "paciente")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegistrarEmocao([FromBody] RegistrarEmocaoRequest request, CancellationToken ct)
    {
        var id = await _registrarEmocao.ExecutarAsync(request, ct);
        return CreatedAtAction(nameof(RegistrarEmocao), new { id }, new { id });
    }

    /// <summary>CSU09 — relatório de presença (psicólogo).</summary>
    [HttpGet("relatorios/presenca")]
    [Authorize(Roles = "psicologo")]
    [ProducesResponseType(typeof(RelatorioPresencaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RelatorioPresenca(
        [FromQuery] DateTime inicio, [FromQuery] DateTime fim,
        [FromQuery] Guid? pacienteId, CancellationToken ct)
        => Ok(await _presenca.ExecutarAsync(inicio, fim, pacienteId, ct));
}
