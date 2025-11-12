# Para ejecutar deberá ejecutar lo siguiente

## Pruebe primero en local

```env
dotnet clean
```

```env
dotnet build
```

## Luego ejecute el docker

```env
docker-compose up --build
```

## Tenga en cuenta

Que si dejo de ejecutar el docker compose, tiene que realizar la siguiente acción

```env
docker-compose down -v
```

## ENDPOINTS DE PRUEBA

POST:
http://localhost:8081/api/producto/

JSON Ejemplo:

```env
{
        "nombre": "Zapatos",
        "descripcion": "Nike",
        "cantidad": 10,
        "correo": "cbustaca@javeriana.edu.co"
}
```

GET:
http://localhost:8081/api/producto/
