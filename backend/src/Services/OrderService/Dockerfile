# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Shared project first (context is backend/src, so Shared is ./Shared)
COPY ["Shared/TechnologyStore.Shared/TechnologyStore.Shared.csproj", "Shared/TechnologyStore.Shared/"]

# Copy OrderService projects
COPY ["Services/OrderService/OrderService.API/OrderService.API.csproj", "Services/OrderService/OrderService.API/"]
COPY ["Services/OrderService/OrderService.Application/OrderService.Application.csproj", "Services/OrderService/OrderService.Application/"]
COPY ["Services/OrderService/OrderService.Domain/OrderService.Domain.csproj", "Services/OrderService/OrderService.Domain/"]
COPY ["Services/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "Services/OrderService/OrderService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Services/OrderService/OrderService.API/OrderService.API.csproj"

# Copy everything else
COPY ["Shared/TechnologyStore.Shared/", "Shared/TechnologyStore.Shared/"]
COPY ["Services/OrderService/OrderService.API/", "Services/OrderService/OrderService.API/"]
COPY ["Services/OrderService/OrderService.Application/", "Services/OrderService/OrderService.Application/"]
COPY ["Services/OrderService/OrderService.Domain/", "Services/OrderService/OrderService.Domain/"]
COPY ["Services/OrderService/OrderService.Infrastructure/", "Services/OrderService/OrderService.Infrastructure/"]

# Build
WORKDIR "/src/Services/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
