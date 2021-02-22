FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /app

COPY ["src/Skuld.API/Skuld.API.csproj", "src/Skuld.API/"]
RUN dotnet restore "src/Skuld.API/Skuld.API.csproj"
COPY . .

WORKDIR ./src/Skuld.API

RUN dotnet build "Skuld.API.csproj" -c Release -o /app/build
RUN dotnet publish "Skuld.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Skuld.API.dll"]