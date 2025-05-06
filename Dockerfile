# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los archivos del proyecto
COPY ["SchoolManager.csproj", "./"]
RUN dotnet restore

# Copiar el resto de los archivos
COPY . .
WORKDIR "/src"

# Compilar la aplicación
RUN dotnet build "SchoolManager.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "SchoolManager.csproj" -c Release -o /app/publish

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Exponer el puerto 80
EXPOSE 80

# Establecer la variable de entorno para el entorno de producción
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "SchoolManager.dll"] 