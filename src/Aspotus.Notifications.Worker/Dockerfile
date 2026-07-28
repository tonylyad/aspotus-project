FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aspotus.Notifications.Worker/Aspotus.Notifications.Worker.csproj", "Aspotus.Notifications.Worker/"]
COPY ["Aspotus.Shared/Aspotus.Shared.csproj", "Aspotus.Shared/"]
RUN dotnet restore "./Aspotus.Notifications.Worker/Aspotus.Notifications.Worker.csproj"
COPY . .
WORKDIR "/src/Aspotus.Notifications.Worker"
RUN dotnet publish "./Aspotus.Notifications.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Aspotus.Notifications.Worker.dll"]
