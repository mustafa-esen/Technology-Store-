FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (Shared + ProductService projects)
COPY ["src/Shared/TechnologyStore.Shared/TechnologyStore.Shared.csproj", "Shared/TechnologyStore.Shared/"]
COPY ["src/Services/ProductService/ProductService.API/ProductService.API.csproj", "Services/ProductService/ProductService.API/"]
COPY ["src/Services/ProductService/ProductService.Application/ProductService.Application.csproj", "Services/ProductService/ProductService.Application/"]
COPY ["src/Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "Services/ProductService/ProductService.Domain/"]
COPY ["src/Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "Services/ProductService/ProductService.Infrastructure/"]
RUN dotnet restore "Services/ProductService/ProductService.API/ProductService.API.csproj"

# Copy source
COPY ["src/Shared/TechnologyStore.Shared/", "Shared/TechnologyStore.Shared/"]
COPY ["src/Services/ProductService/ProductService.API/", "Services/ProductService/ProductService.API/"]
COPY ["src/Services/ProductService/ProductService.Application/", "Services/ProductService/ProductService.Application/"]
COPY ["src/Services/ProductService/ProductService.Domain/", "Services/ProductService/ProductService.Domain/"]
COPY ["src/Services/ProductService/ProductService.Infrastructure/", "Services/ProductService/ProductService.Infrastructure/"]

WORKDIR /src/Services/ProductService/ProductService.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]
