using Microsoft.EntityFrameworkCore;

namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class ServicioBiblioteca
{
    private readonly BibliotecaContext _context;

    public ServicioBiblioteca(BibliotecaContext context)
    {
        _context = context;
    }

    public List<Libro> BuscarLibros(string busqueda)
    {
        return _context.Libros
            .Where(l => l.Titulo.Contains(busqueda) || l.Autor.Contains(busqueda))
            .OrderBy(l => l.Titulo)
            .ToList();
    }

    public int ObtenerCopiasDisponibles(string isbn)
    {
        var libro = _context.Libros.Find(isbn);
        if (libro == null) return -1;
        int prestados = _context.Prestamos
            .Count(p => p.LibroISBN == isbn && (p.EstadoId == 1 || p.EstadoId == 3));
        return libro.CantidadCopias - prestados;
    }

    public (bool valido, string mensaje) ValidarPrestamo(int socioId, string isbn)
    {
        var socio = _context.Socios.Include(s => s.TipoSocio).FirstOrDefault(s => s.NroSocio == socioId);
        if (socio == null) return (false, "Socio no encontrado.");

        if (socio.Activo == 0)
            return (false, "El socio está inactivo. No puede realizar préstamos.");

        bool tieneMultas = _context.Multas.Any(m => m.SocioId == socioId && m.Pagada == 0);
        if (tieneMultas)
            return (false, "El socio tiene multas pendientes. Debe abonarlas para retirar libros.");

        int disponibles = ObtenerCopiasDisponibles(isbn);
        if (disponibles <= 0)
            return (false, "SIN_COPIAS");

        int prestamosActivos = _context.Prestamos
            .Count(p => p.SocioId == socioId && (p.EstadoId == 1 || p.EstadoId == 3));
        if (prestamosActivos >= socio.TipoSocio.MaxLibrosSimultaneos)
            return (false, $"Alcanzó el límite de {socio.TipoSocio.MaxLibrosSimultaneos} libros simultáneos.");

        return (true, "");
    }

    public Prestamo RegistrarPrestamo(int socioId, string isbn)
    {
        var socio = _context.Socios.Include(s => s.TipoSocio).First(s => s.NroSocio == socioId);

        var prestamo = new Prestamo
        {
            SocioId = socioId,
            LibroISBN = isbn,
            FechaPrestamo = DateTime.Now.ToString("yyyy-MM-dd"),
            FechaVencimiento = DateTime.Now.AddDays(socio.TipoSocio.DiasPrestamo).ToString("yyyy-MM-dd"),
            EstadoId = 1,
            Renovado = 0
        };

        _context.Prestamos.Add(prestamo);
        _context.SaveChanges();
        return prestamo;
    }

    public (Prestamo prestamo, Multa? multa) ProcesarDevolucion(int socioId, string isbn)
    {
        var prestamo = _context.Prestamos
            .Include(p => p.Socio).ThenInclude(s => s.TipoSocio)
            .Include(p => p.Libro)
            .FirstOrDefault(p => p.SocioId == socioId && p.LibroISBN == isbn
                && (p.EstadoId == 1 || p.EstadoId == 3));

        if (prestamo == null)
            throw new InvalidOperationException("No se encontró un préstamo activo de este libro para el socio indicado.");

        string fechaDev = DateTime.Now.ToString("yyyy-MM-dd");
        prestamo.FechaDevolucion = fechaDev;
        prestamo.EstadoId = 2;

        Multa? multa = null;
        if (DateTime.Parse(fechaDev) > DateTime.Parse(prestamo.FechaVencimiento))
        {
            int dias = (DateTime.Parse(fechaDev) - DateTime.Parse(prestamo.FechaVencimiento)).Days;
            decimal monto = dias * prestamo.Socio.TipoSocio.MultaPorDia;
            multa = new Multa
            {
                SocioId = socioId,
                PrestamoId = prestamo.Id,
                Monto = monto,
                Pagada = 0
            };
            _context.Multas.Add(multa);
        }

        _context.SaveChanges();

        var reserva = _context.Reservas
            .Include(r => r.Socio)
            .Where(r => r.LibroISBN == isbn && r.EstadoId == 1)
            .OrderBy(r => r.FechaReserva)
            .FirstOrDefault();

        if (reserva != null)
        {
            reserva.EstadoId = 2;
            _context.SaveChanges();
            Console.WriteLine($"\n*** Notificación: La reserva de {reserva.Socio.Nombre} {reserva.Socio.Apellido} para \"{prestamo.Libro.Titulo}\" ha sido cumplida. ***");
        }

        return (prestamo, multa);
    }

    public Reserva? ProcesarReserva(int socioId, string isbn)
    {
        var socio = _context.Socios.Find(socioId);
        if (socio == null)
        {
            Console.WriteLine("Socio no encontrado.");
            return null;
        }

        if (socio.Activo == 0)
        {
            Console.WriteLine("El socio está inactivo. No puede realizar reservas.");
            return null;
        }

        bool yaReservado = _context.Reservas
            .Any(r => r.SocioId == socioId && r.LibroISBN == isbn && r.EstadoId == 1);
        if (yaReservado)
        {
            Console.WriteLine("Ya tiene una reserva activa para este libro.");
            return null;
        }

        var reserva = new Reserva
        {
            SocioId = socioId,
            LibroISBN = isbn,
            FechaReserva = DateTime.Now.ToString("yyyy-MM-dd"),
            EstadoId = 1
        };

        _context.Reservas.Add(reserva);
        _context.SaveChanges();
        return reserva;
    }

    public Socio? ObtenerDetalleSocio(int socioId)
    {
        return _context.Socios
            .Include(s => s.TipoSocio)
            .Include(s => s.Prestamos).ThenInclude(p => p.Libro)
            .Include(s => s.Prestamos).ThenInclude(p => p.Estado)
            .Include(s => s.Multas)
            .FirstOrDefault(s => s.NroSocio == socioId);
    }
}
