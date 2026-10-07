using MediatR;

namespace FloriculturaGestao.Domain.Common;

public abstract class EventoDominio : INotification
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
}
