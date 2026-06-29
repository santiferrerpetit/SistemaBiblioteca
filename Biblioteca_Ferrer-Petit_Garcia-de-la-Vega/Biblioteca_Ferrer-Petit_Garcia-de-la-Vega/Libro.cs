namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class Libro
{
    public string ISBN { get; set; }
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string Genero { get; set; }
    public int CantidadCopias { get; set; }

    public List<Prestamo> Prestamos { get; set; } = new();
    public List<Reserva> Reservas { get; set; } = new();
}
