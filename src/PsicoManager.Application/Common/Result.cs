namespace PsicoManager.Application.Common;

/// <summary>Resultado simples de operação de aplicação.</summary>
public class Result<T>
{
    public bool Sucesso { get; }
    public T? Valor { get; }
    public string? Erro { get; }
    public int StatusHttp { get; }

    private Result(bool sucesso, T? valor, string? erro, int status)
        => (Sucesso, Valor, Erro, StatusHttp) = (sucesso, valor, erro, status);

    public static Result<T> Ok(T valor) => new(true, valor, null, 200);
    public static Result<T> Criado(T valor) => new(true, valor, null, 201);
    public static Result<T> Falha(string erro, int status = 400) => new(false, default, erro, status);
}
