# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Shared project first
COPY ["Shared/TechnologyStore.Shared/TechnologyStore.Shared.csproj", "Shared/TechnologyStore.Shared/"]

# Copy BasketService projects
COPY ["Services/BasketService/BasketService.API/BasketService.API.csproj", "Services/BasketService/BasketService.API/"]
COPY ["Services/BasketService/BasketService.Application/BasketService.Application.csproj", "Services/BasketService/BasketService.Application/"]
COPY ["Services/BasketService/BasketService.Domain/BasketService.Domain.csproj", "Services/BasketService/BasketService.Domain/"]
COPY ["Services/BasketService/BasketService.Infrastructure/BasketService.Infrastructure.csproj", "Services/BasketService/BasketService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/BasketService/BasketService.API/BasketService.API.csproj"

# Copy everything else
COPY ["Shared/TechnologyStore.Shared/", "Shared/TechnologyStore.Shared/"]
COPY ["Services/BasketService/BasketService.API/", "Services/BasketService/BasketService.API/"]
COPY ["Services/BasketService/BasketService.Application/", "Services/BasketService/BasketService.Application/"]
COPY ["Services/BasketService/BasketService.Domain/", "Services/BasketService/BasketService.Domain/"]
COPY ["Services/BasketService/BasketService.Infrastructure/", "Services/BasketService/BasketService.Infrastructure/"]

# Build
WORKDIR "/src/Services/BasketService/BasketService.API"
RUN dotnet build "BasketService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "BasketService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BasketService.API.dll"]
