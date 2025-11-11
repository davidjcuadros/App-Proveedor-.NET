CREATE TABLE producto (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    cantidad INT NOT NULL,
    correo VARCHAR(100) NOT NULL
);

INSERT INTO producto(nombre, descripcion, cantidad, correo) VALUES
('Laptop', 'Laptop gamer', 10, 'cuadrosdavid01@gmail.com'),
('Mouse', 'Mouse logitech', 50, 'cuadrosdavid01@gmail.com' );
