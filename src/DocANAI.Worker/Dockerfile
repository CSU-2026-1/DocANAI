FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DocANAI.Contracts/DocANAI.Contracts.csproj", "src/DocANAI.Contracts/"]
COPY ["src/DocANAI.Persistence/DocANAI.Persistence.csproj", "src/DocANAI.Persistence/"]
COPY ["src/DocANAI.Worker/DocANAI.Worker.csproj", "src/DocANAI.Worker/"]
RUN dotnet nuget locals all --clear
RUN dotnet restore "src/DocANAI.Worker/DocANAI.Worker.csproj"
COPY . .
WORKDIR "/src/src/DocANAI.Worker"
RUN dotnet build "./DocANAI.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DocANAI.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocANAI.Worker.dll"]
