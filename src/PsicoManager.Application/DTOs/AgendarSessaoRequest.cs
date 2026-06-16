namespace PsicoManager.Application.DTOs;

public record AgendarSessaoRequest(
    Guid PacienteId,
    DateTime DataHoraUtc,
    string Tipo,            // "presencial" | "teleconsulta"
    short DuracaoMin = 50);
