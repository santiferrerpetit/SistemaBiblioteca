namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class Prestamo
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string LibroISBN { get; set; }
    public string FechaPrestamo { get; set; }
    public string FechaVencimiento { get; set; }
    public string? FechaDevolucion { get; set; }
    public int EstadoId { get; set; }
    public int Renovado { get; set; }

    public Socio Socio { get; set; }
    public Libro Libro { get; set; }
    public EstadoPrestamo Estado { get; set; }
    public List<Multa> Multas { get; set; } = new();
}
