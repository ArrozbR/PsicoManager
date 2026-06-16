using PsicoManager.Domain.Common;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>Tabela: psicologo (EOBD). Usuário principal do sistema.</summary>
public class Psicologo : EntidadeBase
{
    public string Nome { get; private set; } = null!;
    public string Crp { get; private set; } = null!;        // UK, formato UF/XXXXXX
    public string Email { get; private set; } = null!;      // UK
    public string? Telefone { get; private set; }
    public string SenhaHash { get; private set; } = null!;  // BCrypt/Argon2 — nunca texto claro
    public bool MfaAtivo { get; private set; }
    public bool Ativo { get; private set; } = true;         // soft delete

    private readonly List<Paciente> _pacientes = new();
    public IReadOnlyCollection<Paciente> Pacientes => _pacientes.AsReadOnly();

    protected Psicologo() { } // EF

    public Psicologo(string nome, string crp, string email, string senhaHash, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new RegraDeNegocioException("Nome obrigatório.");
        if (string.IsNullOrWhiteSpace(crp)) throw new RegraDeNegocioException("CRP obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new RegraDeNegocioException("E-mail obrigatório.");
        Nome = nome.Trim();
        Crp = crp.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
        Telefone = telefone;
        MfaAtivo = false;
    }

    public void AtivarMfa() => MfaAtivo = true;
    public void Desativar() => Ativo = false;
}
