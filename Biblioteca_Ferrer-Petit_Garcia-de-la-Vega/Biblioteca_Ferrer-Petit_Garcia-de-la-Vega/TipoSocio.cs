namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class TipoSocio
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public int MaxLibrosSimultaneos { get; set; }
    public int DiasPrestamo { get; set; }
    public decimal MultaPorDia { get; set; }

    public List<Socio> Socios { get; set; } = new();
}
