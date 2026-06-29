namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class Multa
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public int PrestamoId { get; set; }
    public decimal Monto { get; set; }
    public int Pagada { get; set; }

    public Socio Socio { get; set; }
    public Prestamo Prestamo { get; set; }
}
