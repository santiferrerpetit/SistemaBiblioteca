namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class EstadoPrestamo
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    public List<Prestamo> Prestamos { get; set; } = new();
}
