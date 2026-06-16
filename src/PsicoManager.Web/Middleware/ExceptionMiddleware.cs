using System.Text.Json;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Web.Middleware;

/// <summary>
/// Converte DomainException nos códigos HTTP corretos (403/409/422/...) usando
/// o formato ProblemDetails. Demais exceções viram 500 genérico.
/// </summary>
public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await EscreverAsync(context, ex.HttpStatus, ex.Message, ex.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");
            await EscreverAsync(context, 500, "Erro interno do servidor.", "InternalServerError");
        }
    }

    private static async Task EscreverAsync(HttpContext ctx, int status, string detalhe, string tipo)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";
        var problema = new
        {
            type = tipo,
            status,
            detail = detalhe
        };
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(problema));
    }
}
