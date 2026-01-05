# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем решения и проекты
COPY *.sln ./
COPY SAS.Backend.API/*.csproj SAS.Backend.API/
COPY SAS.Backend.Application/*.csproj SAS.Backend.Application/
COPY SAS.Backend.Contracts/*.csproj SAS.Backend.Contracts/
COPY SAS.Backend.Domain/*.csproj SAS.Backend.Domain/
COPY SAS.Backend.Infrastructure/*.csproj SAS.Backend.Infrastructure/

RUN dotnet restore

# Копируем остальной код и публикуем
COPY . .
RUN dotnet publish SAS.Backend.API/SAS.Backend.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

# Render задаёт PORT; слушаем на нём
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080
ENTRYPOINT ["dotnet", "SAS.Backend.API.dll"]