# Build and run FluxOps.Api (Minimal API) on .NET 8

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy project files for restore
COPY FluxOps.Api/FluxOps.Api.csproj FluxOps.Api/
COPY FluxOps.Infrastructure/FluxOps.Infrastructure.csproj FluxOps.Infrastructure/
COPY FluxOps.Application/FluxOps.Application.csproj FluxOps.Application/
COPY FluxOps.Domain/FluxOps.Domain.csproj FluxOps.Domain/
RUN dotnet restore FluxOps.Api/FluxOps.Api.csproj
# Copy source and publish
COPY . .
WORKDIR /src/FluxOps.Api
RUN dotnet publish FluxOps.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FluxOps.Api.dll"]
