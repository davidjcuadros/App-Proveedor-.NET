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

## ENDPOINTS DE PRUEBA

POST:
http://localhost:8081/api/producto/

JSON Ejemplo:

```env
{
        "nombre": "Zapatos",
        "descripcion": "Nike",
        "cantidad": 10,
        "correo": "cuadrosdavid01@gmail.com"
}
```

GET:
http://localhost:8081/api/producto/
