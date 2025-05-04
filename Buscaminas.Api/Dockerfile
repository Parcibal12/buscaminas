# Fase 1: Construcción/Publicación
# Usa una imagen SDK de .NET (ajusta la versión si usas una diferente, ej: 7.0, 8.0)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos .csproj y restaura las dependencias
# Copia solo los .csproj primero para aprovechar el caché de Docker
COPY *.csproj ./
RUN dotnet restore

# Copia el resto del código fuente
COPY . ./

# Publica la aplicación para producción
# Asegúrate de que el nombre del proyecto (Minesweeper.Api.csproj) sea correcto
RUN dotnet publish Minesweeper.Api.csproj -c Release -o /app/publish # <-- Nombre corregido aquí

# Fase 2: Ejecución
# Usa una imagen de runtime de ASP.NET más ligera (ajusta la versión si es necesario)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expone el puerto que usa tu API (usualmente 80 para Kestrel dentro del contenedor)
EXPOSE 80

# Define el punto de entrada para ejecutar la aplicación
# Asegúrate de que el nombre del archivo DLL (Minesweeper.Api.dll) sea correcto
ENTRYPOINT ["dotnet", "Minesweeper.Api.dll"] # <-- Verifica también este nombre DLL