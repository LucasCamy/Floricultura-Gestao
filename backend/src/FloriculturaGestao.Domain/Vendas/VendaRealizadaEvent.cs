using FloriculturaGestao.Domain.Common;

namespace FloriculturaGestao.Domain.Vendas;

public class VendaRealizadaEvent : EventoDominio
{
    public Guid VendaId { get; }
    public Guid ClienteId { get; }
    public decimal ValorTotal { get; }

    public VendaRealizadaEvent(Guid vendaId, Guid clienteId, decimal valorTotal)
    {
        VendaId = vendaId;
        ClienteId = clienteId;
        ValorTotal = valorTotal;
    }
}
