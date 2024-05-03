FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["trb-prefs.csproj", "./"]
RUN dotnet restore "trb-prefs.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "trb-prefs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "trb-prefs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "trb-prefs.dll"]
