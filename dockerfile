# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el archivo .csproj
COPY ProveedorApp/ProveedorApp.csproj ProveedorApp/
RUN dotnet restore ProveedorApp/ProveedorApp.csproj

# Copiamos el resto del código
COPY . .
WORKDIR /src/ProveedorApp
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProveedorApp.dll"]