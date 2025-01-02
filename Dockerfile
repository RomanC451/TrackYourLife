# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "App/TrackYourLife.App.csproj"
RUN dotnet publish "App/TrackYourLife.App.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 5001

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TrackYourLifeDotnet.App.dll"]