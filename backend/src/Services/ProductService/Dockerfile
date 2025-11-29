# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy ProductService projects
COPY ["ProductService/ProductService.API/ProductService.API.csproj", "ProductService/ProductService.API/"]
COPY ["ProductService/ProductService.Application/ProductService.Application.csproj", "ProductService/ProductService.Application/"]
COPY ["ProductService/ProductService.Domain/ProductService.Domain.csproj", "ProductService/ProductService.Domain/"]
COPY ["ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "ProductService/ProductService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "ProductService/ProductService.API/ProductService.API.csproj"

# Copy everything else
COPY ["ProductService/ProductService.API/", "ProductService/ProductService.API/"]
COPY ["ProductService/ProductService.Application/", "ProductService/ProductService.Application/"]
COPY ["ProductService/ProductService.Domain/", "ProductService/ProductService.Domain/"]
COPY ["ProductService/ProductService.Infrastructure/", "ProductService/ProductService.Infrastructure/"]

# Build
WORKDIR "/src/ProductService/ProductService.API"
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
