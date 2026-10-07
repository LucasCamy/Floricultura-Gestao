namespace FloriculturaGestao.Domain.Common;

public abstract class EntidadeBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; protected set; }

    private readonly List<EventoDominio> _eventos = [];
    public IReadOnlyCollection<EventoDominio> Eventos => _eventos.AsReadOnly();

    protected void AdicionarEvento(EventoDominio evento) => _eventos.Add(evento);
    public void LimparEventos() => _eventos.Clear();

    protected void MarcarAtualizado() => AtualizadoEm = DateTime.UtcNow;
}
