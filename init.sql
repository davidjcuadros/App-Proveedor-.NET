CREATE TABLE producto (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    cantidad INT NOT NULL
);

INSERT INTO producto(nombre, descripcion, cantidad) VALUES
('Laptop', 'Laptop gamer', 10),
('Mouse', 'Mouse logitech', 50);
