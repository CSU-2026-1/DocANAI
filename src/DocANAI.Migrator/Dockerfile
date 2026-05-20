FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DocANAI.Migrator/DocANAI.Migrator.csproj", "src/DocANAI.Migrator/"]
COPY ["src/DocANAI.Persistence/DocANAI.Persistence.csproj", "src/DocANAI.Persistence/"]
RUN dotnet restore "src/DocANAI.Migrator/DocANAI.Migrator.csproj"
COPY . .
WORKDIR "/src/src/DocANAI.Migrator"
RUN dotnet build "./DocANAI.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DocANAI.Migrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocANAI.Migrator.dll"]
