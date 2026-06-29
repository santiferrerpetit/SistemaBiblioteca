namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class Reserva
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string LibroISBN { get; set; }
    public string FechaReserva { get; set; }
    public int EstadoId { get; set; }

    public Socio Socio { get; set; }
    public Libro Libro { get; set; }
    public EstadoReserva Estado { get; set; }
}
