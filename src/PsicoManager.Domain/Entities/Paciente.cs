using PsicoManager.Domain.Common;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>Tabela: paciente (EOBD). Usuário secundário, vinculado a um psicólogo.</summary>
public class Paciente : EntidadeBase
{
    public Guid PsicologoId { get; private set; }
    public string Nome { get; private set; } = null!;
    public string? Cpf { get; private set; }              // 11 dígitos, validado na aplicação
    public DateOnly? DataNascimento { get; private set; }  // não pode ser futura
    public string? Email { get; private set; }            // UK por psicologo_id
    public string? Telefone { get; private set; }
    public string? Whatsapp { get; private set; }          // com DDI para API
    public bool Ativo { get; private set; } = true;        // soft delete, retenção 5 anos (RN3)

    protected Paciente() { } // EF

    public Paciente(Guid psicologoId, string nome, string? cpf = null,
        DateOnly? dataNascimento = null, string? email = null,
        string? telefone = null, string? whatsapp = null)
    {
        if (psicologoId == Guid.Empty) throw new RegraDeNegocioException("Psicólogo inválido.");
        if (string.IsNullOrWhiteSpace(nome)) throw new RegraDeNegocioException("Nome obrigatório.");
        if (dataNascimento.HasValue && dataNascimento.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new RegraDeNegocioException("Data de nascimento não pode ser futura.");

        PsicologoId = psicologoId;
        Nome = nome.Trim();
        Cpf = cpf;
        DataNascimento = dataNascimento;
        Email = email?.Trim().ToLowerInvariant();
        Telefone = telefone;
        Whatsapp = whatsapp;
    }

    public void Desativar() => Ativo = false;
}
