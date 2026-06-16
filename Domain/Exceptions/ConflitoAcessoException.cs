namespace PsicoManager.Domain.Exceptions;

/// <summary>
/// RN1 / RNF01 — psicólogo sem vínculo terapêutico tenta acessar prontuário.
/// HTTP 403. O acesso é negado e registrado no log de auditoria.
/// </summary>
public sealed class ConflitoAcessoException : DomainException
{
    public ConflitoAcessoException(string mensagem = "Acesso negado: sem vínculo terapêutico com o paciente.")
        : base(mensagem, 403) { }
}
