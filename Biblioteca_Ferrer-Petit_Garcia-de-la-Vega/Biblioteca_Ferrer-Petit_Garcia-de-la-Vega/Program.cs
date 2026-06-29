using Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

class Program
{
    static void Main(string[] args)
    {
        var context = new BibliotecaContext();
        var servicio = new ServicioBiblioteca(context);

        Console.WriteLine("=== SISTEMA DE GESTIÓN DE BIBLIOTECA ===");

        bool salir = false;
        while (!salir)
        {
            Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
            Console.WriteLine("1. Realizar préstamo");
            Console.WriteLine("2. Realizar devolución");
            Console.WriteLine("3. Reservar libro");
            Console.WriteLine("4. Ver detalle de socio");
            Console.WriteLine("5. Salir");
            Console.Write("\nSeleccione una opción: ");

            string opcion = Console.ReadLine()?.Trim() ?? "";

            switch (opcion)
            {
                case "1": FlujoPrestamo(servicio); break;
                case "2": FlujoDevolucion(servicio); break;
                case "3": FlujoReserva(servicio); break;
                case "4": FlujoDetalleSocio(servicio); break;
                case "5": salir = true; break;
                default: Console.WriteLine("Opción inválida. Intente nuevamente."); break;
            }
        }
    }

    static void FlujoPrestamo(ServicioBiblioteca servicio)
    {
        Console.Write("\nIngrese título o autor del libro: ");
        string busqueda = Console.ReadLine()?.Trim() ?? "";

        var libros = servicio.BuscarLibros(busqueda);
        if (libros.Count == 0)
        {
            Console.WriteLine("No se encontraron libros con ese criterio.");
            return;
        }

        Console.WriteLine("\nLibros encontrados:");
        Console.WriteLine(new string('-', 80));
        foreach (var libro in libros)
        {
            int disp = servicio.ObtenerCopiasDisponibles(libro.ISBN);
            Console.WriteLine($"ISBN: {libro.ISBN}");
            Console.WriteLine($"Título: {libro.Titulo}");
            Console.WriteLine($"Autor: {libro.Autor}");
            Console.WriteLine($"Copias disponibles: {disp}/{libro.CantidadCopias}");
            Console.WriteLine();
        }

        Console.Write("Ingrese el ISBN del libro: ");
        string isbn = Console.ReadLine()?.Trim() ?? "";

        var libroSel = libros.FirstOrDefault(l => l.ISBN == isbn);
        if (libroSel == null)
        {
            Console.WriteLine("ISBN no válido.");
            return;
        }

        Console.Write("Ingrese número de socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int socioId))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var (valido, mensaje) = servicio.ValidarPrestamo(socioId, isbn);

        if (!valido)
        {
            if (mensaje == "SIN_COPIAS")
            {
                Console.Write("No hay copias disponibles. ¿Desea reservar el libro? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() == "s")
                {
                    var reserva = servicio.ProcesarReserva(socioId, isbn);
                    if (reserva != null)
                        Console.WriteLine("Reserva registrada exitosamente.");
                }
            }
            else
            {
                Console.WriteLine(mensaje);
            }
            return;
        }

        Console.Write($"\nConfirmar préstamo de \"{libroSel.Titulo}\" al socio N°{socioId}? (s/n): ");
        if (Console.ReadLine()?.Trim().ToLower() != "s")
        {
            Console.WriteLine("Préstamo cancelado.");
            return;
        }

        var prestamo = servicio.RegistrarPrestamo(socioId, isbn);
        Console.WriteLine("\nPréstamo registrado exitosamente.");
        Console.WriteLine($"Libro: {libroSel.Titulo}");
        Console.WriteLine($"Fecha de préstamo: {prestamo.FechaPrestamo}");
        Console.WriteLine($"Fecha de vencimiento: {prestamo.FechaVencimiento}");
    }

    static void FlujoDevolucion(ServicioBiblioteca servicio)
    {
        Console.Write("\nIngrese número de socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int socioId))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = servicio.ObtenerDetalleSocio(socioId);
        if (socio == null)
        {
            Console.WriteLine("Socio no encontrado.");
            return;
        }

        var activos = socio.Prestamos.Where(p => p.EstadoId == 1 || p.EstadoId == 3).ToList();
        if (activos.Count == 0)
        {
            Console.WriteLine($"El socio {socio.Nombre} {socio.Apellido} no tiene préstamos activos.");
            return;
        }

        Console.WriteLine($"\nPréstamos activos de {socio.Nombre} {socio.Apellido}:");
        for (int i = 0; i < activos.Count; i++)
        {
            var p = activos[i];
            string estado = p.EstadoId == 3 ? "VENCIDO" : "Activo";
            Console.WriteLine($"{i + 1}. {p.Libro.Titulo} (desde {p.FechaPrestamo}, vence: {p.FechaVencimiento}) [{estado}]");
        }

        Console.Write("\nSeleccione el número del libro a devolver: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int idx) || idx < 1 || idx > activos.Count)
        {
            Console.WriteLine("Selección inválida.");
            return;
        }

        var seleccionado = activos[idx - 1];

        try
        {
            var (prestamo, multa) = servicio.ProcesarDevolucion(socioId, seleccionado.LibroISBN);
            Console.WriteLine($"\nDevolución registrada para \"{prestamo.Libro.Titulo}\".");
            Console.WriteLine($"Fecha de devolución: {prestamo.FechaDevolucion}");

            if (multa != null)
            {
                Console.WriteLine($"*** Multa generada: ${multa.Monto:F2} por demora. ***");
            }
            else
            {
                Console.WriteLine("Devolución dentro del plazo. Sin multa.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void FlujoReserva(ServicioBiblioteca servicio)
    {
        Console.Write("\nIngrese título o autor del libro: ");
        string busqueda = Console.ReadLine()?.Trim() ?? "";

        var libros = servicio.BuscarLibros(busqueda);
        if (libros.Count == 0)
        {
            Console.WriteLine("No se encontraron libros con ese criterio.");
            return;
        }

        Console.WriteLine("\nLibros encontrados:");
        Console.WriteLine(new string('-', 80));
        foreach (var libro in libros)
        {
            int disp = servicio.ObtenerCopiasDisponibles(libro.ISBN);
            Console.WriteLine($"ISBN: {libro.ISBN}");
            Console.WriteLine($"Título: {libro.Titulo}");
            Console.WriteLine($"Autor: {libro.Autor}");
            Console.WriteLine($"Copias disponibles: {disp}/{libro.CantidadCopias}");
            Console.WriteLine();
        }

        Console.Write("Ingrese el ISBN del libro: ");
        string isbn = Console.ReadLine()?.Trim() ?? "";

        Console.Write("Ingrese número de socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int socioId))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var reserva = servicio.ProcesarReserva(socioId, isbn);
        if (reserva != null)
        {
            Console.WriteLine("\nReserva registrada exitosamente.");
            Console.WriteLine($"Fecha de reserva: {reserva.FechaReserva}");
        }
    }

    static void FlujoDetalleSocio(ServicioBiblioteca servicio)
    {
        Console.Write("\nIngrese número de socio: ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out int socioId))
        {
            Console.WriteLine("Número de socio inválido.");
            return;
        }

        var socio = servicio.ObtenerDetalleSocio(socioId);
        if (socio == null)
        {
            Console.WriteLine("Socio no encontrado.");
            return;
        }

        Console.WriteLine($"\n=== DATOS DEL SOCIO ===");
        Console.WriteLine($"Nombre: {socio.Nombre} {socio.Apellido}");
        Console.WriteLine($"Email: {socio.Email}");
        Console.WriteLine($"Tipo: {socio.TipoSocio.Nombre}");
        Console.WriteLine($"Estado: {(socio.Activo == 1 ? "Activo" : "Inactivo")}");

        var activos = socio.Prestamos.Where(p => p.EstadoId == 1 || p.EstadoId == 3).ToList();
        Console.WriteLine($"\n--- Préstamos activos ({activos.Count}) ---");
        if (activos.Count == 0)
            Console.WriteLine("No tiene préstamos activos.");
        else
        {
            foreach (var p in activos)
            {
                string estado = p.EstadoId == 3 ? "VENCIDO" : "Activo";
                Console.WriteLine($"  {p.Libro.Titulo} | Desde: {p.FechaPrestamo} | Vence: {p.FechaVencimiento} | {estado}");
            }
        }

        var historial = socio.Prestamos.Where(p => p.EstadoId == 2).ToList();
        Console.WriteLine($"\n--- Historial de devoluciones ({historial.Count}) ---");
        if (historial.Count == 0)
            Console.WriteLine("No hay devoluciones registradas.");
        else
        {
            foreach (var p in historial)
            {
                Console.WriteLine($"  {p.Libro.Titulo} | Prestado: {p.FechaPrestamo} | Devuelto: {p.FechaDevolucion}");
            }
        }

        var multasPendientes = socio.Multas.Where(m => m.Pagada == 0).ToList();
        Console.WriteLine($"\n--- Multas pendientes ({multasPendientes.Count}) ---");
        if (multasPendientes.Count == 0)
            Console.WriteLine("No tiene multas pendientes.");
        else
        {
            foreach (var m in multasPendientes)
            {
                Console.WriteLine($"  ${m.Monto:F2}");
            }
        }
        Console.WriteLine();
    }
}
