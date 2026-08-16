FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Maquinitas.Domain/Maquinitas.Domain.csproj Maquinitas.Domain/
COPY src/Maquinitas.Infrastructure/Maquinitas.Infrastructure.csproj Maquinitas.Infrastructure/
COPY src/Maquinitas.Api/Maquinitas.Api.csproj Maquinitas.Api/
RUN dotnet restore Maquinitas.Api/Maquinitas.Api.csproj

COPY src/ .
RUN dotnet publish Maquinitas.Api/Maquinitas.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Render (and most PaaS hosts) inject the port to listen on via $PORT.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet Maquinitas.Api.dll"]
