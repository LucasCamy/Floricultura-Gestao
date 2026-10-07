namespace FloriculturaGestao.Domain.Auditoria;

public class RegistroAuditoria
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Tabela { get; set; } = string.Empty;
    public string RegistroId { get; set; } = string.Empty;
    public string Acao { get; set; } = string.Empty; // Criou | Atualizou | Excluiu
    public string RealizadoPor { get; set; } = string.Empty;
    public DateTime RealizadoEm { get; set; } = DateTime.UtcNow;
    public string? Detalhes { get; set; }
}
