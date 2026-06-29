namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class EstadoReserva
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    public List<Reserva> Reservas { get; set; } = new();
}
