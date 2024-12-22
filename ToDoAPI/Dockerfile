# Użyj obrazu z .NET ASP.NET (do uruchamiania aplikacji)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Użyj obrazu z pełnym SDK .NET (do budowania aplikacji i uruchamiania migracji)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ToDoAPI/ToDoAPI.csproj", "ToDoAPI/"]
RUN dotnet restore "ToDoAPI/ToDoAPI.csproj"
COPY . . 
WORKDIR "/src/ToDoAPI"
RUN dotnet build "ToDoAPI.csproj" -c Release -o /app/build

# Publikowanie aplikacji
FROM build AS publish
RUN dotnet publish "ToDoAPI.csproj" -c Release -o /app/publish

# Zainstaluj dotnet-ef i wykonaj migracje
FROM build AS migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src/ToDoAPI
RUN dotnet ef migrations add InitialCreate
RUN dotnet ef database update

# Końcowy obraz, tylko z aplikacją
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoAPI.dll"]
