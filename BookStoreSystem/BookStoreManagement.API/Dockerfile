# syntax=docker/dockerfile:1

# Multi-stage Dockerfile for CI build verification.
# It uses official Microsoft .NET images and produces a runnable ASP.NET Core image,
# but the CI workflow only builds it; it does not publish or deploy anything.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# ASP.NET Core container images listen on 8080 by convention.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /src

COPY ["BookStoreManagement.API.csproj", "./"]
RUN dotnet restore "./BookStoreManagement.API.csproj"

FROM restore AS build

COPY . .
RUN dotnet build "./BookStoreManagement.API.csproj" \
    --configuration Release \
    --no-restore

FROM build AS publish
RUN dotnet publish "./BookStoreManagement.API.csproj" \
    --configuration Release \
    --no-build \
    --output /app/publish \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BookStoreManagement.API.dll"]
