-- ============================================================
-- Sistema de Gestión de Biblioteca
-- Base de datos: biblioteca
-- ============================================================

-- Tabla: TipoSocio
CREATE TABLE TipoSocio (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    MaxLibrosSimultaneos INTEGER NOT NULL,
    DiasPrestamo INTEGER NOT NULL,
    MultaPorDia DECIMAL(10,2) NOT NULL
);

-- Tabla: EstadoPrestamo
CREATE TABLE EstadoPrestamo (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL
);

-- Tabla: EstadoReserva
CREATE TABLE EstadoReserva (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL
);

-- Tabla: Libro
CREATE TABLE Libro (
    ISBN TEXT PRIMARY KEY,
    Titulo TEXT NOT NULL,
    Autor TEXT NOT NULL,
    Genero TEXT NOT NULL,
    CantidadCopias INTEGER NOT NULL
);

-- Tabla: Socio
CREATE TABLE Socio (
    NroSocio INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Apellido TEXT NOT NULL,
    Email TEXT NOT NULL,
    TipoSocioId INTEGER NOT NULL,
    Activo INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (TipoSocioId) REFERENCES TipoSocio(Id)
);

-- Tabla: Prestamo
CREATE TABLE Prestamo (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SocioId INTEGER NOT NULL,
    LibroISBN TEXT NOT NULL,
    FechaPrestamo TEXT NOT NULL,
    FechaVencimiento TEXT NOT NULL,
    FechaDevolucion TEXT,
    EstadoId INTEGER NOT NULL,
    Renovado INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (SocioId) REFERENCES Socio(NroSocio),
    FOREIGN KEY (LibroISBN) REFERENCES Libro(ISBN),
    FOREIGN KEY (EstadoId) REFERENCES EstadoPrestamo(Id)
);

-- Tabla: Reserva
CREATE TABLE Reserva (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SocioId INTEGER NOT NULL,
    LibroISBN TEXT NOT NULL,
    FechaReserva TEXT NOT NULL,
    EstadoId INTEGER NOT NULL,
    FOREIGN KEY (SocioId) REFERENCES Socio(NroSocio),
    FOREIGN KEY (LibroISBN) REFERENCES Libro(ISBN),
    FOREIGN KEY (EstadoId) REFERENCES EstadoReserva(Id)
);

-- Tabla: Multa
CREATE TABLE Multa (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SocioId INTEGER NOT NULL,
    PrestamoId INTEGER NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Pagada INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (SocioId) REFERENCES Socio(NroSocio),
    FOREIGN KEY (PrestamoId) REFERENCES Prestamo(Id)
);

-- ============================================================
-- Datos iniciales
-- ============================================================

-- TipoSocio
INSERT INTO TipoSocio (Nombre, MaxLibrosSimultaneos, DiasPrestamo, MultaPorDia) VALUES
('Común', 3, 7, 150.00),
('Estudiante', 5, 14, 75.00),
('Docente', 8, 30, 50.00);

-- EstadoPrestamo
INSERT INTO EstadoPrestamo (Nombre) VALUES
('Activo'),
('Devuelto'),
('Vencido');

-- EstadoReserva
INSERT INTO EstadoReserva (Nombre) VALUES
('Pendiente'),
('Cumplida'),
('Cancelada');

-- Libro (mínimo 5, con varias copias)
INSERT INTO Libro (ISBN, Titulo, Autor, Genero, CantidadCopias) VALUES
('978-987-738-555-7', 'Cien años de soledad', 'Gabriel García Márquez', 'Ficción', 4),
('978-950-07-5040-3', 'El Aleph', 'Jorge Luis Borges', 'Ficción', 2),
('978-987-580-877-4', 'Breve historia del tiempo', 'Stephen Hawking', 'Ciencia', 3),
('978-950-49-3147-8', 'Historia argentina', 'Felipe Pigna', 'Historia', 5),
('978-987-042-518-6', 'Rayuela', 'Julio Cortázar', 'Ficción', 3),
('978-950-03-6078-4', 'El origen de las especies', 'Charles Darwin', 'Ciencia', 2);

-- Socio (mínimo 5, de distintos tipos)
INSERT INTO Socio (Nombre, Apellido, Email, TipoSocioId, Activo) VALUES
('Lucía', 'Martínez', 'lucia.martinez@email.com', 1, 1),
('Carlos', 'González', 'carlos.gonzalez@email.com', 2, 1),
('María', 'Fernández', 'maria.fernandez@email.com', 3, 1),
('Pedro', 'López', 'pedro.lopez@email.com', 1, 0),
('Ana', 'Rodríguez', 'ana.rodriguez@email.com', 2, 1),
('Javier', 'Díaz', 'javier.diaz@email.com', 3, 1);

-- Prestamos (algunos activos, otros vencidos)
INSERT INTO Prestamo (SocioId, LibroISBN, FechaPrestamo, FechaVencimiento, FechaDevolucion, EstadoId, Renovado) VALUES
-- Préstamo activo (Lucía)
(1, '978-987-738-555-7', '2025-06-25', '2025-07-02', NULL, 1, 0),
-- Préstamo vencido (Carlos)
(2, '978-950-07-5040-3', '2025-06-01', '2025-06-15', NULL, 3, 0),
-- Préstamo devuelto (María)
(3, '978-987-580-877-4', '2025-05-20', '2025-06-19', '2025-06-18', 2, 0),
-- Préstamo activo (Ana)
(5, '978-950-49-3147-8', '2025-06-26', '2025-07-10', NULL, 1, 0),
-- Préstamo vencido (Javier)
(6, '978-987-042-518-6', '2025-05-15', '2025-06-14', NULL, 3, 0),
-- Préstamo devuelto (Carlos)
(2, '978-987-738-555-7', '2025-05-01', '2025-05-15', '2025-05-14', 2, 0),
-- Préstamo activo (Lucía) — segundo préstamo
(1, '978-950-03-6078-4', '2025-06-27', '2025-07-04', NULL, 1, 0);

-- Reservas
INSERT INTO Reserva (SocioId, LibroISBN, FechaReserva, EstadoId) VALUES
-- Reserva pendiente de Pedro sobre El Aleph (que está prestado y vencido)
(4, '978-950-07-5040-3', '2025-06-20', 1),
-- Reserva cumplida de Javier
(6, '978-987-738-555-7', '2025-05-20', 2),
-- Reserva cancelada de Ana
(5, '978-987-580-877-4', '2025-06-10', 3);

-- Multas
INSERT INTO Multa (SocioId, PrestamoId, Monto, Pagada) VALUES
(2, 2, 1050.00, 0);
