# ===============================
# Base image runtime
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Criar pasta para SQLite com permissões corretas
RUN mkdir -p /app/data && chmod -R 777 /app/data

# Expor portas da API
EXPOSE 8080
EXPOSE 8081

# Trocar para usuário não-root depois de criar pastas
USER $APP_UID

# ===============================
# Build stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar projetos para o build
COPY ["InvestimentosJwtApi/InvestimentosJwtApi.csproj", "InvestimentosJwtApi/"]
COPY ["InvestimentosJwt.Application/InvestimentosJwt.Application.csproj", "InvestimentosJwt.Application/"]
COPY ["InvestimentosJwt.Domain/InvestimentosJwt.Domain.csproj", "InvestimentosJwt.Domain/"]
COPY ["InvestimentosJwt.Infra.Data/InvestimentosJwt.Infra.Data.csproj", "InvestimentosJwt.Infra.Data/"]

# Restaurar dependências
RUN dotnet restore "./InvestimentosJwtApi/InvestimentosJwtApi.csproj"

# Copiar todo o restante do código
COPY . .

WORKDIR "/src/InvestimentosJwtApi"

# Build da solução
RUN dotnet build "./InvestimentosJwtApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===============================
# Publish stage
# ===============================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./InvestimentosJwtApi.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

# ===============================
# Final stage
# ===============================
FROM base AS final
WORKDIR /app

# Copiar arquivos publicados do stage publish
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "InvestimentosJwtApi.dll"]
