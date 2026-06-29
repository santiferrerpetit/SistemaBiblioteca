using Microsoft.EntityFrameworkCore;

namespace Biblioteca_Ferrer_Petit_Garcia_de_la_Vega;

public class BibliotecaContext : DbContext
{
    public DbSet<TipoSocio> TiposSocio { get; set; }
    public DbSet<EstadoPrestamo> EstadosPrestamo { get; set; }
    public DbSet<EstadoReserva> EstadosReserva { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Socio> Socios { get; set; }
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Multa> Multas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=biblioteca.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoSocio>(entity =>
        {
            entity.ToTable("TipoSocio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.MultaPorDia).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<EstadoPrestamo>(entity =>
        {
            entity.ToTable("EstadoPrestamo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
        });

        modelBuilder.Entity<EstadoReserva>(entity =>
        {
            entity.ToTable("EstadoReserva");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired();
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.ToTable("Libro");
            entity.HasKey(e => e.ISBN);
            entity.Property(e => e.Titulo).IsRequired();
            entity.Property(e => e.Autor).IsRequired();
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.ToTable("Socio");
            entity.HasKey(e => e.NroSocio);
            entity.Property(e => e.Nombre).IsRequired();
            entity.Property(e => e.Apellido).IsRequired();
            entity.HasOne(e => e.TipoSocio)
                  .WithMany(t => t.Socios)
                  .HasForeignKey(e => e.TipoSocioId);
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.ToTable("Prestamo");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Socio)
                  .WithMany(s => s.Prestamos)
                  .HasForeignKey(e => e.SocioId);
            entity.HasOne(e => e.Libro)
                  .WithMany(l => l.Prestamos)
                  .HasForeignKey(e => e.LibroISBN);
            entity.HasOne(e => e.Estado)
                  .WithMany(e => e.Prestamos)
                  .HasForeignKey(e => e.EstadoId);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("Reserva");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Socio)
                  .WithMany(s => s.Reservas)
                  .HasForeignKey(e => e.SocioId);
            entity.HasOne(e => e.Libro)
                  .WithMany(l => l.Reservas)
                  .HasForeignKey(e => e.LibroISBN);
            entity.HasOne(e => e.Estado)
                  .WithMany(e => e.Reservas)
                  .HasForeignKey(e => e.EstadoId);
        });

        modelBuilder.Entity<Multa>(entity =>
        {
            entity.ToTable("Multa");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Socio)
                  .WithMany(s => s.Multas)
                  .HasForeignKey(e => e.SocioId);
            entity.HasOne(e => e.Prestamo)
                  .WithMany(p => p.Multas)
                  .HasForeignKey(e => e.PrestamoId);
        });
    }
}
