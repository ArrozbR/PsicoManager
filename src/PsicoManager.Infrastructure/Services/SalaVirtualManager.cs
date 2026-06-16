using Microsoft.Extensions.Configuration;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Services;

/// <summary>
/// Gera links E2EE de teleconsulta (RNF02 / Res. CFP 11/2018). O token único
/// (UUID) compõe a URL base configurada em Teleconsulta:BaseUrl.
/// </summary>
public sealed class SalaVirtualManager : ISalaVirtualManager
{
    private readonly string _baseUrl;

    public SalaVirtualManager(IConfiguration config)
        => _baseUrl = config["Teleconsulta:BaseUrl"] ?? "https://meet.psicomanager.app/sala";

    public string CriarSala(Guid sessaoId)
    {
        var token = Guid.NewGuid().ToString("N");
        return $"{_baseUrl}/{token}";
    }
}
