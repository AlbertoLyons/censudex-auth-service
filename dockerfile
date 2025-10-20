FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /auth-service-app

EXPOSE 80
EXPOSE 5003

# COPY csproj and restore as distinct layers
COPY ./*csproj ./
RUN dotnet restore

# COPY everything else and build app
COPY . .
RUN dotnet publish -c Release -o out

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /auth-service-app
COPY --from=build /auth-service-app/out .
ENTRYPOINT ["dotnet", "censudex-auth-service.dll"]