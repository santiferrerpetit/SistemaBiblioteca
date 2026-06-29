using Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

class Program
{
    static void Main(string[] args)
    {
        var context = new BibliotecaContext();

        var libros = context.Libros
            .OrderBy(l => l.Titulo)
            .ToList();

        Console.WriteLine("=== Libros disponibles en la biblioteca ===\n");

        foreach (var libro in libros)
        {
            int prestados = context.Prestamos
                .Count(p => p.LibroISBN == libro.ISBN && (p.EstadoId == 1 || p.EstadoId == 3));

            int disponibles = libro.CantidadCopias - prestados;

            Console.WriteLine($"ISBN: {libro.ISBN}");
            Console.WriteLine($"Título: {libro.Titulo}");
            Console.WriteLine($"Autor: {libro.Autor}");
            Console.WriteLine($"Género: {libro.Genero}");
            Console.WriteLine($"Copias totales: {libro.CantidadCopias} | Disponibles: {disponibles}");
            Console.WriteLine();
        }
    }
}
