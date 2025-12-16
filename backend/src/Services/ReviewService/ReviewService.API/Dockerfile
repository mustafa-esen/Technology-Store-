FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Services/ReviewService/ReviewService.API/ReviewService.API.csproj", "Services/ReviewService/ReviewService.API/"]
COPY ["Services/ReviewService/ReviewService.Application/ReviewService.Application.csproj", "Services/ReviewService/ReviewService.Application/"]
COPY ["Services/ReviewService/ReviewService.Domain/ReviewService.Domain.csproj", "Services/ReviewService/ReviewService.Domain/"]
COPY ["Services/ReviewService/ReviewService.Infrastructure/ReviewService.Infrastructure.csproj", "Services/ReviewService/ReviewService.Infrastructure/"]

RUN dotnet restore "Services/ReviewService/ReviewService.API/ReviewService.API.csproj"

# Copy all source files
COPY . .

WORKDIR "/src/Services/ReviewService/ReviewService.API"
RUN dotnet build "ReviewService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReviewService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewService.API.dll"]
