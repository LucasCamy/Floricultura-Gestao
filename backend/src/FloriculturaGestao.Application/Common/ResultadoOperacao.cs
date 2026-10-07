namespace FloriculturaGestao.Application.Common;

public class ResultadoOperacao<T>
{
    public bool Sucesso { get; }
    public T? Dados { get; }
    public string? Mensagem { get; }
    public List<string> Erros { get; } = [];

    private ResultadoOperacao(bool sucesso, T? dados, string? mensagem, List<string>? erros = null)
    {
        Sucesso = sucesso;
        Dados = dados;
        Mensagem = mensagem;
        if (erros is not null) Erros = erros;
    }

    public static ResultadoOperacao<T> Ok(T dados, string? mensagem = null) => new(true, dados, mensagem);
    public static ResultadoOperacao<T> Falha(string mensagem) => new(false, default, mensagem);
    public static ResultadoOperacao<T> Falha(List<string> erros) => new(false, default, "Ocorreram erros de validação.", erros);
}

public class ResultadoOperacao
{
    public bool Sucesso { get; }
    public string? Mensagem { get; }
    public List<string> Erros { get; } = [];

    private ResultadoOperacao(bool sucesso, string? mensagem, List<string>? erros = null)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
        if (erros is not null) Erros = erros;
    }

    public static ResultadoOperacao Ok(string? mensagem = null) => new(true, mensagem);
    public static ResultadoOperacao Falha(string mensagem) => new(false, mensagem);
    public static ResultadoOperacao Falha(List<string> erros) => new(false, "Ocorreram erros de validação.", erros);
}
