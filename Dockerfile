# This phase is used when running in VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# This phase is used to compile the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AutoDoc.csproj", "."]
RUN dotnet restore "./AutoDoc.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./AutoDoc.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This phase is used to publish the service project to be copied to the final phase
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AutoDoc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This phase is used in production or when running in VS in normal mode (default when not using Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoDoc.dll"]