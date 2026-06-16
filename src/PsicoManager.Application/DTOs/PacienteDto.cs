namespace PsicoManager.Application.DTOs;

public record PacienteDto(Guid Id, string Nome, string? Email, string? Whatsapp, bool Ativo);
