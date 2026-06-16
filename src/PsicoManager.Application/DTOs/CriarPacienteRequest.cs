namespace PsicoManager.Application.DTOs;

public record CriarPacienteRequest(
    string Nome, string? Cpf = null, DateOnly? DataNascimento = null,
    string? Email = null, string? Telefone = null, string? Whatsapp = null);
