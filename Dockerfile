# ----------------------------------------------------
# Базовый runtime-образ
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# ----------------------------------------------------
# Сборка приложения
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["SAS.Backend.API/SAS.Backend.API.csproj", "SAS.Backend.API/"]
COPY ["SAS.Backend.Application/SAS.Backend.Application.csproj", "SAS.Backend.Application/"]
COPY ["SAS.Backend.Contracts/SAS.Backend.Contracts.csproj", "SAS.Backend.Contracts/"]
COPY ["SAS.Backend.Domain/SAS.Backend.Domain.csproj", "SAS.Backend.Domain/"]
COPY ["SAS.Backend.Infrastructure/SAS.Backend.Infrastructure.csproj", "SAS.Backend.Infrastructure/"]

RUN dotnet restore "SAS.Backend.API/SAS.Backend.API.csproj"

COPY . .

WORKDIR "/src/SAS.Backend.API"

RUN dotnet build \
    "SAS.Backend.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# ----------------------------------------------------
# Публикация
# ----------------------------------------------------
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish \
    "SAS.Backend.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# ----------------------------------------------------
# Финальный образ
# ----------------------------------------------------
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SAS.Backend.API.dll"]