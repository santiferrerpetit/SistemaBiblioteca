namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class Socio
{
    public int NroSocio { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public int TipoSocioId { get; set; }
    public int Activo { get; set; }

    public TipoSocio TipoSocio { get; set; }
    public List<Prestamo> Prestamos { get; set; } = new();
    public List<Reserva> Reservas { get; set; } = new();
    public List<Multa> Multas { get; set; } = new();
}
