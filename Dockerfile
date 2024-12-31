# Build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_HTTP_PORTS=5001
# ENV ASPNETCORE_HTTPS_PORTS=7001
ENV ASPNETCORE_URLS=http://+:5001 
#;https://+:7001
ENV ASPNETCORE_ENVIRONMENT=Development
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert/cert.pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=Waryor.001


EXPOSE 5001
#  7001


WORKDIR /app
COPY publish/ .
# COPY cert/ cert/

ENTRYPOINT [ "dotnet", "TrackYourLifeDotnet.App.dll" ]


# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# # Copy csproj and restore dependencies
# COPY ["App/TrackYourLife.App.csproj", "TrackYourLife.App/"]
# RUN dotnet restore "TrackYourLife.App/TrackYourLife.App.csproj"

# # Copy and publish app
# COPY . .
# WORKDIR "/src/TrackYourLife.App"
# RUN dotnet publish "TrackYourLife.App.csproj" -c Release -o /app/publish

# # Runtime stage
# FROM mcr.microsoft.com/dotnet/aspnet:8.0
# WORKDIR /app
# COPY --from=build /app/publish .

# ENV ASPNETCORE_URLS=http://+:5000
# EXPOSE 5000

# ENTRYPOINT ["dotnet", "TrackYourLife.App.dll"]