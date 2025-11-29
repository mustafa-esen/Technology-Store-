FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files (need Shared + all IdentityService projects)
COPY ["src/Shared/TechnologyStore.Shared/TechnologyStore.Shared.csproj", "Shared/TechnologyStore.Shared/"]
COPY ["src/Services/IdentityService/IdentityService.API/IdentityService.API.csproj", "Services/IdentityService/IdentityService.API/"]
COPY ["src/Services/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "Services/IdentityService/IdentityService.Application/"]
COPY ["src/Services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "Services/IdentityService/IdentityService.Domain/"]
COPY ["src/Services/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "Services/IdentityService/IdentityService.Infrastructure/"]
COPY ["src/Services/IdentityService/IdentityService.Tests/IdentityService.Tests.csproj", "Services/IdentityService/IdentityService.Tests/"]
RUN dotnet restore "Services/IdentityService/IdentityService.API/IdentityService.API.csproj"

# Copy source
COPY ["src/Shared/TechnologyStore.Shared/", "Shared/TechnologyStore.Shared/"]
COPY ["src/Services/IdentityService/IdentityService.API/", "Services/IdentityService/IdentityService.API/"]
COPY ["src/Services/IdentityService/IdentityService.Application/", "Services/IdentityService/IdentityService.Application/"]
COPY ["src/Services/IdentityService/IdentityService.Domain/", "Services/IdentityService/IdentityService.Domain/"]
COPY ["src/Services/IdentityService/IdentityService.Infrastructure/", "Services/IdentityService/IdentityService.Infrastructure/"]
COPY ["src/Services/IdentityService/IdentityService.Tests/", "Services/IdentityService/IdentityService.Tests/"]

WORKDIR /src/Services/IdentityService/IdentityService.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.API.dll"]
