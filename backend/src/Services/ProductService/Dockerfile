# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ProductService projects
COPY ["Services/ProductService/ProductService.API/ProductService.API.csproj", "Services/ProductService/ProductService.API/"]
COPY ["Services/ProductService/ProductService.Application/ProductService.Application.csproj", "Services/ProductService/ProductService.Application/"]
COPY ["Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "Services/ProductService/ProductService.Domain/"]
COPY ["Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "Services/ProductService/ProductService.Infrastructure/"]

# Copy Shared library
COPY ["Shared/TechnologyStore.Shared/TechnologyStore.Shared.csproj", "Shared/TechnologyStore.Shared/"]

# Restore dependencies
RUN dotnet restore "Services/ProductService/ProductService.API/ProductService.API.csproj"

# Copy everything else
COPY ["Services/ProductService/ProductService.API/", "Services/ProductService/ProductService.API/"]
COPY ["Services/ProductService/ProductService.Application/", "Services/ProductService/ProductService.Application/"]
COPY ["Services/ProductService/ProductService.Domain/", "Services/ProductService/ProductService.Domain/"]
COPY ["Services/ProductService/ProductService.Infrastructure/", "Services/ProductService/ProductService.Infrastructure/"]
COPY ["Shared/TechnologyStore.Shared/", "Shared/TechnologyStore.Shared/"]

# Build
WORKDIR "/src/Services/ProductService/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
