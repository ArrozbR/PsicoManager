namespace PsicoManager.Domain.Services;

/// <summary>Gera salas E2EE de teleconsulta (RNF02 / Res. CFP 11/2018).</summary>
public interface ISalaVirtualManager
{
    /// <summary>Cria uma sala única e retorna o link de acesso.</summary>
    string CriarSala(Guid sessaoId);
}
